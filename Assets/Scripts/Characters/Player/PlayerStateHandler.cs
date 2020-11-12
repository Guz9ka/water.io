using System.Collections;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour, ICharacterStateHandler
{
    private Player _player;

    [Header("Находится ли игрок на земле")]
    [SerializeField]
    private Transform groundChecker;
    [SerializeField]
    private float groudCheckDistance;
    private LayerMask groundMask;

    [Header("Проверка тайла спереди игрока")]
    [SerializeField]
    private Vector3 tileCheckOffset;
    [SerializeField]
    private float tileCheckRadius;

    [SerializeField]
    private float tileJumpDelay;
    private bool tileJumpAvailable = true;

    //[Header("Смена состояний игрока")]
    public delegate void PlayerDeath();
    public event PlayerDeath OnPlayerDied;

    public delegate void PlayerRevive();
    public event PlayerRevive OnPlayerRevive;

    public delegate void PlayerVictory();
    public event PlayerVictory OnPlayerWin;

    private void Start()
    {
        _player = GetComponent<Player>();

        OnPlayerDied += PlayerDied;
        OnPlayerRevive += PlayerRevived;
        OnPlayerWin += PlayerWon;

        groundMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        GroundCheck();
        JumpCheck();
    }

    #region Проверка смен состояния игрока
    public void GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(groundChecker.position, groudCheckDistance, groundMask);
        bool inAir = _player.PlayerAction != PlayerCurrentAction.TileJump &&
            _player.PlayerAction != PlayerCurrentAction.FlyingForward &&
            _player.PlayerAction != PlayerCurrentAction.FlyingUp;

        if (isGrounded && _player.PlayerAction == PlayerCurrentAction.Fall)
        {
            _player.PlayerAction = PlayerCurrentAction.Run;
            _player.ResetCharacteristics();
        }
        else if (!isGrounded && inAir)
        {
            _player.PlayerAction = PlayerCurrentAction.Fall;
        }
    }

    Vector3 tileCheckPosition;
    public void JumpCheck()
    {
        tileCheckPosition = gameObject.transform.position + tileCheckOffset;
        bool tileLost = !Physics.CheckSphere(tileCheckPosition, tileCheckRadius, groundMask); //true, когда перед игроком нет тайла

        if (tileLost && tileJumpAvailable && _player.PlayerAction == PlayerCurrentAction.Run)
        {
            StartCoroutine(TileJumpSwitch());
            _player.PlayerAction = PlayerCurrentAction.TileJump;
        }
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

    public IEnumerator TileJumpSwitch()
    {
        tileJumpAvailable = false;

        yield return new WaitForSeconds(tileJumpDelay);

        tileJumpAvailable = true;
    }
    #endregion

    #region Ответ на смену состояний игрока
    public void PlayerDied()
    {
        _player.PlayerState = PlayerState.Dead;

        _player.ResetCharacteristics();
    }

    public void PlayerRevived()
    {
        _player.PlayerState = PlayerState.Alive;

        _player.ResetCharacteristics();
    }

    public void PlayerWon()
    {
        _player.PlayerState = PlayerState.Win;

        _player.MoveSpeed = 0;
        _player.JoystickSensitivity = 0;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

    #region Реакция на смену состояния игры
    public void OnGameStart()
    {
        _player.PlayerState = PlayerState.Alive;
    }

    public void OnGameEnd()
    {
        _player.PlayerState = PlayerState.Menus;
    }
    #endregion
}
