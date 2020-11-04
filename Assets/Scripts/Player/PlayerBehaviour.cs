using System.Collections;
using UnityEngine;

enum PlayerCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    Jump,
    Slide
}

public class PlayerBehaviour : Player, IJumpable
{
    [Header("Положения и состояния игрока")]
    public PlayerState playerState;
    private PlayerCurrentAction playerAction;

    [Header("Локальные параметры игрока")]
    public CharacterController controller;
    protected GameObject player;

    [Header("Параметры джойстика")]
    public FloatingJoystick joystick;

    public float joystickjoystickSensitivityOriginal; //базовая чувствительность джойстика, к которой он потом может вернуться
    private float joystickSensitivity; //чувствительность джойстика в данный момент
    public float joystickSensitivitySlide;

    [Header("Находится ли игрок на земле")]
    public Transform groundChecker;
    public float groudCheckDistance;
    private LayerMask groundMask;

    [Header("Проверка тайла спереди игрока")]
    public Vector3 tileCheckOffset;
    public float tileCheckRadius;

    public float tileJumpDelay;
    public bool tileJumpAvailable;

    //[Header("Смена состояний игрока")]
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

        //Events
        OnPlayerDied += PlayerDied;
        OnPlayerRevive += PlayerRevived;
        OnPlayerWin += PlayerWon;

        ResetCharacteristics();

        groundMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        if (playerState == PlayerState.Alive)
        {
            // observing for player actions
            GroundCheck();
            JumpCheck();

            // execute movement tasks
            Move();
        }
    }

    #region Проверка смен состояния игрока
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

    Vector3 tileCheckPosition;
    void JumpCheck()
    {
        tileCheckPosition = player.transform.position + tileCheckOffset;
        bool tileLost = !Physics.CheckSphere(tileCheckPosition, tileCheckRadius, groundMask); //true, когда перед игроком нет тайла

        if(tileLost && tileJumpAvailable && playerAction != PlayerCurrentAction.Jump && playerAction != PlayerCurrentAction.Fall)
        {
            StartCoroutine(JumpSwitch());
            playerAction = PlayerCurrentAction.Jump;
        }
    }
    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(tileCheckPosition, tileCheckRadius);
    }

    #region Выполнение движения

    protected override void Move()
    {
        Run();

        switch (playerAction)
        {
            case PlayerCurrentAction.Run:
                //выполняется всегда, когда игрок жив
                break;
            case PlayerCurrentAction.Fall:
                Fall();
                break;
            case PlayerCurrentAction.Jump:
                Jump();
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
    }

    protected override void PlayerRevived()
    {
        playerState = PlayerState.Alive;

        ResetCharacteristics();
    }

    protected override void PlayerWon()
    {
        playerState = PlayerState.Win;

        playerMoveSpeed = 0;
        joystickSensitivity = 0;
        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

    #region Реакция на смену состояния игры
    public void OnGameStart()
    {
    }

    public void OnGameEnd()
    {

    }
    #endregion

    private void ResetCharacteristics()
    {
        playerMoveSpeed = playerMoveSpeedOriginal;
        joystickSensitivity = joystickjoystickSensitivityOriginal;

        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    IEnumerator JumpSwitch()
    {
        tileJumpAvailable = false;

        yield return new WaitForSeconds(tileJumpDelay);

        tileJumpAvailable = true;
    }
}
