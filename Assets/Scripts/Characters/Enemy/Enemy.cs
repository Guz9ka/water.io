using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyCurrentTask
{
    FindTrampoline,
    RunToTrampoline,
    RunForward
}

public enum EnemyCurrentAction
{
    Run, //стандартное состояние, когда нет других команд
    Fall, //игрок в этом состоянии, когда падает вниз
    Jump,
    Slide
}

public class Enemy : Character
{
    [Header("Параметры ИИ противника")]
    public NavMeshAgent Agent;
    [SerializeField]
    private GameObject endZone;

    public float moveForwardDistance;
    public CharacterController controller;

    private Vector3 destination;

    [Header("Поиск ближайшего батута")] //когда состояние воды - прилив
    public float searchRadius;
    private int trampolineMask;

    [Header("Состояния противника")]
    public EnemyCurrentAction EnemyAction;
    public EnemyCurrentTask EnemyTask;
    public CharacterState EnemyState;


    void Start()
    {
        EnemyAction = new EnemyCurrentAction();
        EnemyState = new CharacterState();
        EnemyTask = new EnemyCurrentTask();

        trampolineMask = LayerMask.GetMask("Trampoline");
    }

    void FixedUpdate()
    {
        if(EnemyState == CharacterState.Alive)
        {
            ResetSkinRotation();

            if (Agent.enabled)
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
        Vector3 searchPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + searchRadius + 2);
        Collider[] hitInfo = Physics.OverlapSphere(searchPosition, searchRadius, trampolineMask);

        Vector3 trampolinePosition = new Vector3(1000, 1000, 1000);
        foreach (var target in hitInfo)
        {
            if(target.transform.position.y - transform.position.y < trampolinePosition.y - transform.position.y && target.transform.position.z - transform.position.z < trampolinePosition.z - transform.position.z)
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

        switch (EnemyAction)
        {
            case EnemyCurrentAction.Fall:
                Fall();
                break;
            case EnemyCurrentAction.Jump:
                TileJump();
                break;
        }
    }

    void ExecuteTask()
    {
        Vector3 enemyPosition = transform.position;

        switch (EnemyTask)
        {
            case EnemyCurrentTask.FindTrampoline:
                destination = FindTrampoline();
                EnemyTask = EnemyCurrentTask.RunToTrampoline;
                break;
            case EnemyCurrentTask.RunToTrampoline:
                // Ничего не происходит, т.к. противник уже выполнил FindTrampoline() и начал двигаться к нему.
                // Так нужно, чтобы игрок каждый раз не искал заново батут, а лишь продолжал двигаться к уже назначеной цели
                break;
            case EnemyCurrentTask.RunForward:
                destination = new Vector3(enemyPosition.x, enemyPosition.y, enemyPosition.z + moveForwardDistance);
                break;
        }

        if (Agent.enabled == true) { Agent.SetDestination(destination); }
    }

    protected override void Run()
    {
        Vector3 moveHorizontal = transform.forward * Agent.speed;
        controller.Move(moveHorizontal * Time.deltaTime);
    }

    protected override void TileJump()
    {
        Velocity.y = Mathf.Sqrt(JumpHeight * -2 * gravity);
        Velocity.y += gravity * Time.deltaTime;

        controller.Move(Velocity * Time.deltaTime);

        EnemyAction = EnemyCurrentAction.Fall;
    }

    protected override void Fall()
    {
        Velocity.y += gravity * Time.deltaTime;

        controller.Move(Velocity * Time.deltaTime);
    }
    #endregion

    #region Сброс состояний игрока
    public void ResetCharacteristics()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    void ResetSkinRotation()
    {
        ChatacterSkin.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    void ResetRotation()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion

}
