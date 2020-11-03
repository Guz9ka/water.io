using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyCurrentTask
{
    FindTrampoline,
    RunToTrampoline,
    RunForward
}

enum EnemyCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    Slide
}

public class EnemyBehaviour : Player, IJumpable
{
    private GameObject enemy;

    [Header("ИИ противника")]
    public NavMeshAgent agent;
    public GameObject endZone;

    public float moveForwardDistance;
    public CharacterController controller;

    private Vector3 destination;

    [Header("Поиск ближайшего батута")] //когда состояние воды - прилив
    public float searchRadius;
    private int trampolineMask;

    [Header("Проверка наличия земли под противником")]
    public Transform groundChecker;
    public float groudCheckDistance;
    private LayerMask groundMask;

    [Header("Состояния противника")]
    private EnemyCurrentAction enemyAction;
    public EnemyCurrentTask enemyTask;
    public PlayerState enemyState;

    //[Header("Смена состояний противника")]
    public delegate void EnemyDeath();
    public event EnemyDeath OnEnemyDied;

    public delegate void EnemyRevive();
    public event EnemyRevive OnEnemyRevive;

    public delegate void EnemyVictory();
    public event EnemyVictory OnEnemyWin;

    void Start()
    {
        enemy = gameObject;   
        DeathReason = null;

        OnEnemyDied += EnemyDied;
        OnEnemyRevive += EnemyRevived;
        OnEnemyWin += EnemyWon;

        enemyAction = new EnemyCurrentAction();
        enemyState = new PlayerState();
        enemyTask = new EnemyCurrentTask();

        groundMask = LayerMask.GetMask("Ground");
        trampolineMask = LayerMask.GetMask("Trampoline");
    }

    void FixedUpdate()
    {
        CheckGround();

        if(enemyState == PlayerState.Alive)
        {
            CheckTrampoline();
            ResetSkinRotation();
            if (agent.enabled)
            {
                ExecuteTask();
            }
            else
            {
                Move();
            }
        }
    }

    #region Действия противника
    private Vector3 FindTrampoline()
    {
        Vector3 searchPosition = new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z + searchRadius + 2);
        Collider[] hitInfo = Physics.OverlapSphere(searchPosition, searchRadius, trampolineMask);

        Vector3 trampolinePosition = new Vector3(1000, 1000, 1000);
        foreach (var target in hitInfo)
        {
            if(target.transform.position.y - enemy.transform.position.y < trampolinePosition.y -  enemy.transform.position.y && target.transform.position.z - enemy.transform.position.z < trampolinePosition.z - enemy.transform.position.z)
            {
                trampolinePosition = target.transform.position;
            }
        }

        return trampolinePosition;
    }

    protected override void Move()
    {
        Run();
        ResetRotation();

        switch (enemyAction)
        {
            case EnemyCurrentAction.Run:
                break;
            case EnemyCurrentAction.Fall:
                Fall();
                break;
        }
    }

    void ExecuteTask()
    {
        Vector3 enemyPosition = enemy.transform.position;

        switch (enemyTask)
        {
            case EnemyCurrentTask.FindTrampoline:
                destination = FindTrampoline();
                enemyTask = EnemyCurrentTask.RunToTrampoline;
                break;
            case EnemyCurrentTask.RunToTrampoline:
                // Ничего не происходит, т.к. противник уже выполнил FindTrampoline() и начал двигаться к нему.
                // Так нужно, чтобы игрок каждый раз не искал заново батут, а лишь продолжал двигаться к уже назначеной цели
                break;
            case EnemyCurrentTask.RunForward:
                destination = new Vector3(enemyPosition.x, enemyPosition.y, enemyPosition.z + moveForwardDistance);
                break;
        }

        if (agent.enabled == true) { agent.SetDestination(destination); }
    }

    protected override void Run()
    {
        Vector3 moveHorizontal = transform.forward * agent.speed;
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

        agent.enabled = false;
        enemyAction = EnemyCurrentAction.Fall;
    }

    public override void SlideOnSlide(Vector3 PlayerRotation)
    {
        enemyAction = EnemyCurrentAction.Slide;

        enemy.transform.rotation = Quaternion.Euler(PlayerRotation);
    }

    #endregion

    #region Проверка состояний противника
    void CheckGround()
    {
        bool isGrounded = Physics.CheckSphere(groundChecker.position, groudCheckDistance, groundMask);

        if (isGrounded == false)
        {
            agent.enabled = false;
            enemyAction = EnemyCurrentAction.Fall;
        }
        else if (isGrounded == true && enemyAction == EnemyCurrentAction.Fall)
        {
            agent.enabled = true;
            enemyTask = EnemyCurrentTask.FindTrampoline;
            enemyAction = EnemyCurrentAction.Run;
        }
    }

    void CheckTrampoline()
    {
        bool OnTrampoline = Physics.CheckSphere(groundChecker.position, groudCheckDistance, trampolineMask);
        
        if(OnTrampoline)
        {
            enemyTask = EnemyCurrentTask.RunForward;
        }
    }
    #endregion

    #region Триггеры смены состояний противника
    public void TriggerDeathEvent(string deathReason)
    {
        DeathReason = deathReason;
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
    protected void EnemyDied()
    {
        enemyState = PlayerState.Dead;
        agent.enabled = false;
        ResetCharacteristics();
    }

    protected void EnemyRevived()
    {
        enemyState = PlayerState.Alive;
        enemyTask = EnemyCurrentTask.RunForward;
        ResetCharacteristics();
    }

    protected void EnemyWon()
    {
        enemyState = PlayerState.Win;
        //после победы игрок должен пройти немного вперед
    }
    #endregion

    #region Сброс состояний игрока
    private void ResetCharacteristics()
    {
        enemy.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    void ResetSkinRotation()
    {
        playerSkin.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    void ResetRotation()
    {
        enemy.transform.rotation = Quaternion.Euler(Vector3.zero);
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
