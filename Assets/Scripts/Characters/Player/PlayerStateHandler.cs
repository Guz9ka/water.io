using System.Collections;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour, ICharacterStateHandler
{
    private Player _player;

    [Header("Находится ли игрок на земле")]
    [SerializeField]
    private Transform _groundChecker;
    [SerializeField]
    private float _groudCheckDistance;
    private LayerMask _groundMask;

    [Header("Проверка тайла спереди игрока")]
    [SerializeField]
    private Vector3 _tileCheckOffset;
    [SerializeField]
    private float _tileCheckRadius;

    [SerializeField]
    private float _tileJumpDelay;
    private bool _tileJumpAvailable = true;

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

        _groundMask = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        GroundCheck();
        TileJumpCheck();
    }

    #region Проверка смен состояния игрока
    public void GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(_groundChecker.position, _groudCheckDistance, _groundMask);
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

    private Vector3 _tileCheckPosition;
    public void TileJumpCheck()
    {
        _tileCheckPosition = gameObject.transform.position + _tileCheckOffset;
        bool tileLost = !Physics.CheckSphere(_tileCheckPosition, _tileCheckRadius, _groundMask); //true, когда перед игроком нет тайла

        if (tileLost && _tileJumpAvailable && _player.PlayerAction == PlayerCurrentAction.Run)
        {
            StartCoroutine(TileJumpSwitch());
            _player.PlayerAction = PlayerCurrentAction.TileJump;
        }
    }
    #endregion
    
    #region Триггеры смены состояний игрока
    public void TriggerDeathEvent() //bug
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
        _tileJumpAvailable = false;

        yield return new WaitForSeconds(_tileJumpDelay);

        _tileJumpAvailable = true;
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
