using System.Collections;
using UnityEngine;

public enum PlayerCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    Jump,
    Slide,
    FlyingUp,
    FlyingForward
}

public class PlayerActions : Player, IJumpable
{
    [Header("Положения и состояния игрока")]
    public PlayerState playerState;
    public PlayerCurrentAction PlayerAction;

    [Header("Локальные параметры игрока")]
    public CharacterController controller;
    protected GameObject player;

    [Header("Параметры джойстика")]
    public FloatingJoystick joystick;

    [SerializeField]
    private float joystickSensitivity; //чувствительность джойстика в данный момент
    public float joystickSensitivitySlide;

    [Header("Бустеры")]
    public IJetPack jetPack;
    public ISpeedBooster speedBooster;

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
        jetPack = GetComponent<IJetPack>();
        speedBooster = GetComponent<ISpeedBooster>();

        //states
        player = gameObject;
        playerState = new PlayerState();
        PlayerAction = new PlayerCurrentAction();

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
    bool GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(groundChecker.position, groudCheckDistance, groundMask);

        if (isGrounded == false && PlayerAction != PlayerCurrentAction.Jump && PlayerAction != PlayerCurrentAction.FlyingForward && PlayerAction != PlayerCurrentAction.FlyingUp)
        {
            PlayerAction = PlayerCurrentAction.Fall;
        }
        else if (isGrounded == true && PlayerAction == PlayerCurrentAction.Fall)
        {
            PlayerAction = PlayerCurrentAction.Run;
        }

        return isGrounded;
    }

    Vector3 tileCheckPosition;
    void JumpCheck()
    {
        tileCheckPosition = player.transform.position + tileCheckOffset;
        bool tileLost = !Physics.CheckSphere(tileCheckPosition, tileCheckRadius, groundMask); //true, когда перед игроком нет тайла

        if (tileLost && tileJumpAvailable && PlayerAction == PlayerCurrentAction.Run)
        {
            StartCoroutine(JumpSwitch());
            PlayerAction = PlayerCurrentAction.Jump;
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
        switch (PlayerAction)
        {
            case PlayerCurrentAction.Run:
                Run();
                break;
            case PlayerCurrentAction.Fall:
                Fall();
                Run();
                break;
            case PlayerCurrentAction.Jump:
                Jump();
                Run();
                break;
            case PlayerCurrentAction.Slide:
                //выполняется через скрипт горки, нужно переделывать
                break;
            case PlayerCurrentAction.FlyingUp:
                Run();
                jetPack.FlyUp();
                break;
            case PlayerCurrentAction.FlyingForward:
                jetPack.FlyForward();
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

        PlayerAction = PlayerCurrentAction.Fall;
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

        PlayerAction = PlayerCurrentAction.Fall;
    }

    public override void SlideOnSlide(Vector3 PlayerRotation)
    {
        PlayerAction = PlayerCurrentAction.Slide;

        player.transform.rotation = Quaternion.Euler(PlayerRotation);
        playerMoveSpeed = playerSlideSpeed;
        joystickSensitivity = joystickSensitivitySlide;

        //добавить визуальный поворот скина
    }
    #endregion

    #region Триггеры смены состояний игрока
    public void TriggerDeathEvent()
    {
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

    IEnumerator JumpSwitch()
    {
        tileJumpAvailable = false;

        yield return new WaitForSeconds(tileJumpDelay);

        tileJumpAvailable = true;
    }

    private void ResetCharacteristics()
    {
        playerMoveSpeed = playerSpeedOriginal;

        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
