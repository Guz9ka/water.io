using System.Collections;
using UnityEngine;

enum SwipeState
{
    Crawl,
    Jump,
    MoveLeft,
    MoveRight,
    Nothing //состояние, если игрок не сделал никаких свайпов
}

enum PlayerCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    MoveLeft,
    MoveRight,
    Fall, //игрок в этом состоянии, когда падает вниз
    Jump,
    Slide,
    Crawl
}

public class PlayerBehaviour : Player, IJumpable
{
    [Header("Положения и состояния игрока")]
    public PlayerState playerState;
    private PlayerCurrentAction playerAction;
    private SwipeState swipeState;

    [Header("Смещение джойстика для вызова свайпа")] //насколько нужно подвинуть джойстик в ту или иную сторону для начала действия
    public float distanceToMove;

    [Header("Движение по сторонам")]
    public float moveHorizontalSpeed;
    public float moveHorizontalDistance;
    public float moveHorizontalDelay;

    [Header("Параметры джойстика")]
    public FloatingJoystick joystick;

    [Header("Локальные параметры игрока")]
    public CharacterController controller;
    protected GameObject player;

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
        swipeState = new SwipeState();

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
            CheckInput();

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

    private void CheckInput()
    {
        if (playerAction == PlayerCurrentAction.Run)
        {
             swipeState = GetCurrentSwipe();

            switch (swipeState)
            {
                case SwipeState.Crawl:
                    playerAction = PlayerCurrentAction.Crawl;
                    break;
                case SwipeState.Jump:
                    playerAction = PlayerCurrentAction.Jump;
                    break;
                case SwipeState.MoveLeft:
                    playerAction = PlayerCurrentAction.MoveLeft;
                    StartCoroutine(MoveSidesSwitch());
                    break;
                case SwipeState.MoveRight:
                    playerAction = PlayerCurrentAction.MoveRight;
                    StartCoroutine(MoveSidesSwitch());
                    break;
                case SwipeState.Nothing:
                    break;
                default:
                    break;
            }
        }
    }
    #endregion

    #region Считывание свайпов
    SwipeState GetCurrentSwipe()
    {
        bool leftClean = !Physics.Raycast(player.transform.position, Vector3.left, moveHorizontalDistance, LayerMask.GetMask("Wall"));
        bool rightClean = !Physics.Raycast(player.transform.position, Vector3.right, moveHorizontalDistance, LayerMask.GetMask("Wall"));
        
        //Vertical
        if (joystick.Vertical > distanceToMove)
        {
            return SwipeState.Jump;
        }
        else if (joystick.Vertical < -distanceToMove)
        {
            return SwipeState.Crawl;
        }
        //Horizontal
        else if (joystick.Horizontal > distanceToMove && rightClean) //можно добавить больше плавности, если ограничить по вертикали и добавить булевую
        {
            return SwipeState.MoveRight;
        }
        else if (joystick.Horizontal < -distanceToMove && leftClean)
        {
            return SwipeState.MoveLeft;
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
                //выполняется всегда, когда игрок жив
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
            case PlayerCurrentAction.MoveLeft:
                MoveSides(PlayerCurrentAction.MoveLeft);
                break;
            case PlayerCurrentAction.MoveRight:
                MoveSides(PlayerCurrentAction.MoveRight);
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
        Vector3 moveForward = transform.forward * playerMoveSpeed;
        controller.Move(moveForward * Time.deltaTime);
    }

    private IEnumerator MoveSidesSwitch()
    {
        yield return new WaitForSeconds(moveHorizontalDelay);

        playerAction = PlayerCurrentAction.Run;
    }

    private void MoveSides(PlayerCurrentAction moveDirection)
    {
        switch (moveDirection)
        {
            case PlayerCurrentAction.MoveLeft:
                controller.Move(Vector3.left * moveHorizontalSpeed * Time.deltaTime);
                break;
            case PlayerCurrentAction.MoveRight:
                controller.Move(Vector3.right * moveHorizontalSpeed * Time.deltaTime);
                break;
        }
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

        switch (DeathReason)
        {
            case "Obstacle":
                player.transform.position = player.GetComponent<PositionRestore>().RestorePosition(DeathReason);
                break;
            case "Water":
                player.transform.position = player.GetComponent<CheckPointRecorder>().GetLastCheckPoint();
                break;
        }

        DeathReason = null;

        Invoke("TriggerReviveEvent", reviveDelay);
    }

    protected override void PlayerRevived()
    {
        playerState = PlayerState.Alive;

        ResetCharacteristics();
    }

    protected override void PlayerWon()
    {
        playerState = PlayerState.Win;

        controller.Move(new Vector3(0, 0, Mathf.Lerp(0, moveAfterWin, playerMoveSpeed * Time.deltaTime)));

        playerMoveSpeed = 0;
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

        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
