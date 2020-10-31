using System.Collections;
using System.Collections.Generic;
//using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyCurrentTask
{
    FindTrampoline,
    RunToTrampoline,
    RunForward,
    RunToEnd
}

enum EnemyCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    Jump,
    Slide,
    Crawl
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

    [Header("Смена состояний противника")]
    public float reviveDelay;

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
        WaterTide.singleton.OnTideRaising += ChooseEnemyTask;
        WaterTide.singleton.OnTideLowering += ChooseEnemyTask;

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
        OnGameStart();
        if(enemyState == PlayerState.Alive)
        {
            CheckTrampoline();
            Move();

            if(agent.enabled == false && enemyAction != EnemyCurrentAction.Fall) { CheckRunForwardState(); }
        }
    }

    void ChooseEnemyTask() // вызывается при смене состояния прилива
    {
        switch (WaterTide.waterState)
        {
            case WaterState.Raising: // двигаться к батуту
                if (enemyTask != EnemyCurrentTask.FindTrampoline && enemyTask != EnemyCurrentTask.RunToTrampoline && enemyTask != EnemyCurrentTask.RunForward) { enemyTask = EnemyCurrentTask.FindTrampoline; }
                break;
            case WaterState.High: //просто двигаться вперед
                enemyTask = EnemyCurrentTask.RunForward;
                break;
            case WaterState.Lowering: // бежать к концу
                enemyTask = EnemyCurrentTask.RunToEnd;
                break;
            case WaterState.Low: // бежать к концу
                enemyTask = EnemyCurrentTask.RunToEnd;
                break;
        }
    }

    #region Действия противника
    private Vector3 FindTrampoline()
    {
        Vector3 searchPosition = new Vector3(enemy.transform.position.x, enemy.transform.position.y, enemy.transform.position.z + searchRadius + 2);
        var hitInfo = Physics.OverlapSphere(searchPosition, searchRadius, trampolineMask);

        return hitInfo[0].gameObject.transform.position;
    }

    protected override void Move()
    {
        playerSkin.transform.rotation = enemy.transform.rotation = Quaternion.Euler(Vector3.zero);

        switch (enemyAction)
        {
            case EnemyCurrentAction.Run:
                // Направление дается из SetDestination, это же состояние означает что игрок бежит вперед
                if (enemy.transform.position.y > 2) { Run(); }
                break;
            case EnemyCurrentAction.Fall:
                enemy.transform.rotation = Quaternion.Euler(Vector3.zero);
                Fall();
                Run();
                break;
            case EnemyCurrentAction.Jump:
                enemy.transform.rotation = Quaternion.Euler(Vector3.zero);
                Jump();
                Run();
                break;
            case EnemyCurrentAction.Slide:
                // Вызывается из EnemyCollisionReactor
                break;
            case EnemyCurrentAction.Crawl:
                Crawl();
                break;
        }

        Vector3 enemyPosition = enemy.transform.position;

        switch (enemyTask)
        {
            case EnemyCurrentTask.RunToEnd:
                destination = endZone.transform.position;
                break;
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

    protected override void Jump()
    {
        CheckRunForwardState();

        velocity.y = Mathf.Sqrt(playerJumpHeight * -2 * gravity);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        enemyAction = EnemyCurrentAction.Fall;
    }

    protected override void Crawl()
    {
        enemyAction = EnemyCurrentAction.Crawl;
        //задержка

        enemyAction = EnemyCurrentAction.Run;
    }

    protected override void Fall()
    {
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public override void JumpOnTrampoline(float jumpForce) //триггерится через батут
    {
        CheckRunForwardState();
        Debug.Log("jump");
        velocity.y = Mathf.Sqrt(jumpForce * -2 * gravity);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

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

        if (isGrounded == false && enemyAction != EnemyCurrentAction.Jump)
        {
            enemyAction = EnemyCurrentAction.Fall;
        }
        else if (isGrounded == true && enemyAction == EnemyCurrentAction.Jump || enemyAction == EnemyCurrentAction.Fall)
        {
            enemyAction = EnemyCurrentAction.Run;
        }
    }

    void CheckTrampoline()
    {
        bool OnTrampoline = Physics.CheckSphere(groundChecker.position, groudCheckDistance, trampolineMask);
        //Debug.Log("tramp " + OnTrampoline);
        if(OnTrampoline && enemyTask == EnemyCurrentTask.RunToTrampoline)
        {
            enemyTask = EnemyCurrentTask.RunForward;
        }
    }

    void CheckRunForwardState()
    {
        if (agent.enabled == true)
        {
            agent.enabled = false;
            Debug.Log("disabled");
        }
        else if (enemy.transform.position.y < 2)
        {
            agent.enabled = true;
            agent.SetDestination(destination);
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

        switch (DeathReason)
        {
            case "Obstacle":
                enemy.transform.position = enemy.GetComponent<PositionRestore>().RestorePosition(DeathReason);
                break;
            case "Water":
                enemy.transform.position = enemy.GetComponent<CheckPointRecorder>().GetLastCheckPoint();
                break;
        }
        
        DeathReason = null;

        Invoke("TriggerReviveEvent", reviveDelay);
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

    private void ResetCharacteristics()
    {
        enemy.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    #region Реакция на смену состояния игры
    public void OnGameStart()
    {
        ChooseEnemyTask();
    }

    public void OnGameEnd()
    {

    }
    #endregion
}
