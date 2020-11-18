using System.Collections;
using UnityEngine;

public enum PlayerCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    TileJump,
    JumpOnBoots,
    JumpOnTrampoline,
    FlyingUp,
    FlyingForward
}

public class Player : Character
{
    [Header("Положения и состояния игрока")]
    public CharacterState PlayerState;
    public PlayerCurrentAction PlayerAction;

    [Header("Локальные параметры игрока")]
    [HideInInspector]
    public CharacterController Controller;

    [Header("Параметры джойстика")]
    public FloatingJoystick Joystick;

    [HideInInspector]
    public float JoystickSensitivity; //чувствительность джойстика в данный момент
    [SerializeField]
    private float joystickSensitivityDefault; 

    private void Start()
    {
        Controller = GetComponent<CharacterController>();
        ResetCharacteristics();

        //states
        PlayerState = new CharacterState();
        PlayerAction = new PlayerCurrentAction();
    }

    private void FixedUpdate()
    {

        if (PlayerState == CharacterState.Alive)
        {
            Move();
        }
    }

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
            case PlayerCurrentAction.TileJump:
                TileJump();
                Run();
                break;
            case PlayerCurrentAction.JumpOnBoots:
                Run();
                break;
            case PlayerCurrentAction.FlyingUp:
                Run();
                break;
        }
    }

    #region Возможные действия игрока
    protected override void Run()
    {
        float moveSides = Joystick.Horizontal * JoystickSensitivity; //шлепнул прямой инпут т.к. другого вида ввода не предусматривается

        Vector3 moveHorizontal = transform.forward * MoveSpeed + transform.right * moveSides;
        Controller.Move(moveHorizontal * Time.deltaTime);
    }

    protected override void Fall()
    {
        Velocity.y += gravity * Time.deltaTime;
        Controller.Move(Velocity * Time.deltaTime);
    }

    protected override void TileJump()
    {
        Velocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
        Velocity.y += gravity * Time.deltaTime;

        Controller.Move(Velocity * Time.deltaTime);

        PlayerAction = PlayerCurrentAction.Fall;
    }
    #endregion

    public void ResetCharacteristics()
    {
        Velocity = Vector3.zero;
        MoveSpeed = MoveSpeedDefault;
        JoystickSensitivity = joystickSensitivityDefault;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
