using System.Collections;
using UnityEngine;

public enum PlayerState
{
    Alive,
    Dead,
    Win,
    Menus
}

public enum PlayerCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    Jump,
    JumpOnBoots,
    JumpOnTrampoline,
    FlyingUp,
    FlyingForward
}

public class Player : Character
{
    [Header("Положения и состояния игрока")]
    public PlayerState PlayerState;
    public PlayerCurrentAction PlayerAction;

    [Header("Локальные параметры игрока")]
    [HideInInspector]
    public CharacterController controller;

    [Header("Параметры джойстика")]
    public FloatingJoystick joystick;

    [SerializeField]
    public float joystickSensitivity; //чувствительность джойстика в данный момент

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        character = gameObject;
        ResetCharacteristics();

        //states
        PlayerState = new PlayerState();
        PlayerAction = new PlayerCurrentAction();
    }

    private void FixedUpdate()
    {
        if (PlayerState == PlayerState.Alive)
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
            case PlayerCurrentAction.Jump:
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
    protected override void TileJump()
    {
        Velocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
        Velocity.y += gravity * Time.deltaTime;

        controller.Move(Velocity * Time.deltaTime);

        PlayerAction = PlayerCurrentAction.Fall;
    }

    protected override void Run()
    {
        float moveSides = joystick.Horizontal * joystickSensitivity; //шлепнул прямой инпут т.к. другого вида ввода не предусматривается

        Vector3 moveHorizontal = transform.forward * MoveSpeed + transform.right * moveSides;
        controller.Move(moveHorizontal * Time.deltaTime);
    }

    protected override void Fall()
    {
        Velocity.y += gravity * Time.deltaTime;
        controller.Move(Velocity * Time.deltaTime);
    }
    #endregion

    public void ResetCharacteristics()
    {
        Velocity = Vector3.zero;
        MoveSpeed = OriginalSpeed;
        character.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
