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

        OnPlayerDied += CharacterDied;
        OnPlayerRevive += CharacterRevived;
        OnPlayerWin += CharacterWon;

        groundMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        GroundCheck();
        TileJumpCheck();
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
    public void TileJumpCheck()
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
    public void CharacterDied()
    {
        _player.PlayerState = CharacterState.Dead;

        _player.ResetCharacteristics();
    }

    public void CharacterRevived()
    {
        _player.PlayerState = CharacterState.Alive;

        _player.ResetCharacteristics();
    }

    public void CharacterWon()
    {
        _player.PlayerState = CharacterState.Win;

        _player.MoveSpeed = 0;
        _player.JoystickSensitivity = 0;
        gameObject.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

    #region Реакция на смену состояния игры
    public void OnGameStart()
    {
        _player.PlayerState = CharacterState.Alive;
    }

    public void OnGameEnd()
    {
        _player.PlayerState = CharacterState.Menus;
    }
    #endregion
}
