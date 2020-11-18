using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateHandler : MonoBehaviour, ICharacterStateHandler
{
    private Enemy _enemy;

    [Header("Проверка наличия земли под противником")]
    public Transform groundChecker;
    public float groudCheckDistance;
    private LayerMask groundMask;

    [Header("Проверка тайла спереди противника")]
    public Vector3 tileCheckOffset;
    public float tileCheckRadius;

    public float tileJumpDelay;
    public bool tileJumpAvailable;

    //[Header("Смена состояний противника")]
    public delegate void EnemyDeath();
    public event EnemyDeath OnEnemyDied;

    public delegate void EnemyRevive();
    public event EnemyRevive OnEnemyRevive;

    public delegate void EnemyVictory();
    public event EnemyVictory OnEnemyWin;

    private void Start()
    {
        _enemy = GetComponent<Enemy>();

        OnEnemyDied += CharacterDied;
        OnEnemyRevive += CharacterRevived;
        OnEnemyWin += CharacterWon;

        groundMask = LayerMask.GetMask("Ground");
    }

    void Update()
    {
        GroundCheck();

        CheckTrampoline();
        TileJumpCheck();
    }

    #region Проверка состояний противника
    public void GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(groundChecker.position, groudCheckDistance, groundMask);

        if (isGrounded == false)
        {
            _enemy.Agent.enabled = false;
            _enemy.EnemyAction = EnemyCurrentAction.Fall;
        }
        else if (isGrounded == true && _enemy.EnemyAction == EnemyCurrentAction.Fall)
        {
            _enemy.Agent.enabled = true;
            _enemy.EnemyTask = EnemyCurrentTask.FindTrampoline;
            _enemy.EnemyAction = EnemyCurrentAction.Run;
        }
    }

    public void CheckTrampoline()
    {
        bool OnTrampoline = Physics.CheckSphere(groundChecker.position, groudCheckDistance, LayerMask.GetMask("Trampoline"));

        if (OnTrampoline)
        {
            _enemy.EnemyTask = EnemyCurrentTask.RunForward;
        }
    }

    Vector3 tileCheckPosition;
    public void TileJumpCheck()
    {
        tileCheckPosition = _enemy.transform.position + tileCheckOffset;
        bool tileLost = !Physics.CheckSphere(tileCheckPosition, tileCheckRadius, groundMask); //true, когда перед игроком нет тайла

        if (tileLost && tileJumpAvailable && _enemy.EnemyAction != EnemyCurrentAction.Jump && _enemy.EnemyAction != EnemyCurrentAction.Fall)
        {
            StartCoroutine(TileJumpSwitch());
            _enemy.Agent.enabled = false;
            _enemy.EnemyAction = EnemyCurrentAction.Jump;
        }
    }
    #endregion


    public IEnumerator TileJumpSwitch()
    {
        tileJumpAvailable = false;

        yield return new WaitForSeconds(tileJumpDelay);

        tileJumpAvailable = true;
    }

    #region Триггеры смены состояний противника
    public void TriggerDeathEvent()
    {
        OnEnemyDied.Invoke();
    }

    public void TriggerReviveEvent()
    {
        OnEnemyRevive.Invoke();
    }

    public void TriggerWinEvent()
    {
        OnEnemyWin.Invoke();
    }
    #endregion

    #region Ответ на смену состояний игрока
    public void CharacterDied()
    {
        _enemy.EnemyState = CharacterState.Dead;
        _enemy.Agent.enabled = false;
        _enemy.ResetCharacteristics();
    }

    public void CharacterRevived()
    {
        _enemy.EnemyState = CharacterState.Alive;
        _enemy.EnemyTask = EnemyCurrentTask.RunForward;
        _enemy.ResetCharacteristics();
    }

    public void CharacterWon()
    {
        _enemy.EnemyState = CharacterState.Win;
        //после победы игрок должен пройти немного вперед
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
}
