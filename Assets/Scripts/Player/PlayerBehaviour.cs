using System;
using System.Threading;
using UnityEngine;

enum SwipeState
{
    Crawl,
    Jump,
    Nothing //состояние, если игрок не сделал никаких свайпов
}

public class PlayerBehaviour : Player, IJumpable
{
    [Header("Локальные параметры игрока")]
    public CharacterController controller;

    [Header("Параметры джойстика")]
    public FloatingJoystick joystick;

    public float joystickjoystickSensitivityOriginal; //базовая чувствительность джойстика, к которой он потом может вернуться
    private float joystickSensitivity; //чувствительность джойстика в данный момент
    public float joystickSensitivitySlide;

    [Header("Смещение джойстика для вызова свайпа")] //насколько нужно подвинуть джойстик в ту или иную сторону для начала действия
    public float distanceToJump; //вверх для прыжка
    public float distanceToCrawl; //для рывка вниз

    [Header("Находится ли игрок на земле")]
    public Transform groundChecker;
    public float groudCheckDistance;
    private LayerMask groundMask;

    [Header("Смена состояний игрока")]
    public float reviveDelay;

    public delegate void PlayerDeath();
    public event PlayerDeath OnPlayerDied;

    public delegate void PlayerRevive();
    public event PlayerRevive OnPlayerRevive;

    public delegate void PlayerVictory();
    public event PlayerVictory OnPlayerWin;

    private void Start()
    {
        //states
        player = gameObject;
        DeathReason = null;
        playerState = new PlayerState();
        playerAction = new PlayerCurrentAction();
        playerState = PlayerState.Alive;

        //Events
        OnPlayerDied += PlayerDied;
        OnPlayerRevive += PlayerRevived;
        OnPlayerWin += PlayerWinned;

        ResetCharacteristics();

        groundMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Alive)
        {
            // observing for player actions
            GroundCheck(); 
            CheckInput();

            // execute movement tasks
            Move();
        }
    }

    #region Проверка смен состояния игрока
    private void CheckInput()
    {
        SwipeState swipeVertical = GetVerticalSwipe();

        if (playerAction == PlayerCurrentAction.Run)
        {
            switch (swipeVertical)
            {
                case SwipeState.Crawl:
                    playerAction = PlayerCurrentAction.Crawl;
                    break;
                case SwipeState.Jump:
                    playerAction = PlayerCurrentAction.Jump;
                    break;
                case SwipeState.Nothing:
                    break;
            }
        }
    }

    void GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(groundChecker.position, groudCheckDistance, groundMask);

        if (isGrounded == false && playerAction != PlayerCurrentAction.Jump)
        {
            playerAction = PlayerCurrentAction.Fall;
        }
        else if (isGrounded == true && playerAction == PlayerCurrentAction.Jump || playerAction == PlayerCurrentAction.Fall)
        {
            playerAction = PlayerCurrentAction.Run;
        }
    }
    #endregion

    #region Считывание свайпов
    SwipeState GetVerticalSwipe()
    {
        if (joystick.Vertical > distanceToJump)
        {
            return SwipeState.Jump;
        }
        else if (joystick.Vertical < distanceToCrawl)
        {
            return SwipeState.Crawl;
        }
        else
        {
            return SwipeState.Nothing;
        }
    }
    #endregion

    #region Выполнение движения

    protected override void Move()
    {
        Run();

        switch (playerAction)
        {
            case PlayerCurrentAction.Run:
                //выполняется всегда
                break;
            case PlayerCurrentAction.Fall:
                Fall();
                break;
            case PlayerCurrentAction.Jump:
                Jump();
                break;
            case PlayerCurrentAction.Crawl:
                Crawl();
                break;
            case PlayerCurrentAction.Slide:
                //выполняется через скрипт горки
                break;
        }
    }
    #endregion

    #region Возможные действия игрока
    protected override void Jump() 
    {
        velocity.y = Mathf.Sqrt(playerJumpHeight * -2 * gravity);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        playerAction = PlayerCurrentAction.Fall;
    }

    protected override void Crawl()
    {
        playerAction = PlayerCurrentAction.Crawl;
        controller.height = 1.2f;
        //задержка

        playerAction = PlayerCurrentAction.Run;
    }

    protected override void Run()
    {
        float moveSides = joystick.Horizontal * joystickSensitivity;

        Vector3 moveHorizontal = transform.forward * playerMoveSpeed + transform.right * moveSides;
        controller.Move(moveHorizontal * Time.deltaTime);
    }

    protected override void Fall()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public override void JumpOnTrampoline(float jumpForce) //триггерится через батут
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        playerAction = PlayerCurrentAction.Fall;
    }

    public override void SlideOnSlide(Vector3 PlayerRotation)
    {
        playerAction = PlayerCurrentAction.Slide;

        player.transform.rotation = Quaternion.Euler(PlayerRotation);
        playerMoveSpeed = playerSlideSpeed;
        joystickSensitivity = joystickSensitivitySlide;

        //добавить визуальный поворот скина
    }
    #endregion

    #region Триггеры смены состояний игрока
    public void TriggerDeathEvent(string deathReason)
    {
        DeathReason = deathReason;
        OnPlayerDied.Invoke();
    } 

    public void TriggerReviveEvent()
    {
        OnPlayerRevive.Invoke();
    }

    public void TriggerWinEvent()
    {
        OnPlayerWin.Invoke();
    }
    #endregion

    #region Ответ на смену состояний игрока
    protected override void PlayerDied()
    {
        playerState = PlayerState.Dead;

        ResetCharacteristics();

        player.transform.position = player.GetComponent<PositionRestore>().RestorePosition(DeathReason);
        DeathReason = null;

        Invoke("TriggerReviveEvent", reviveDelay);
    }

    protected override void PlayerRevived()
    {
        playerState = PlayerState.Alive;

        ResetCharacteristics();
    }

    protected override void PlayerWinned()
    {
        playerState = PlayerState.Win;

        controller.Move(new Vector3(0, 0, Mathf.Lerp(0, moveAfterWin, playerMoveSpeed * Time.deltaTime)));

        playerMoveSpeed = 0;
        joystickSensitivity = 0;
        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

    private void ResetCharacteristics()
    {
        playerMoveSpeed = playerMoveSpeedOriginal;
        joystickSensitivity = joystickjoystickSensitivityOriginal;

        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
