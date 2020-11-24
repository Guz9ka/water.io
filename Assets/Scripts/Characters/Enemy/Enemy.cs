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
    private GameObject _endZone;

    [SerializeField]
    private float _moveForwardDistance;

    private Vector3 _destination;

    [Header("Поиск ближайшего батута")] //когда состояние воды - прилив
    [SerializeField]
    private float _searchRadius;
    private int _trampolineMask;

    [Header("Состояния противника")]
    public EnemyCurrentAction EnemyAction;
    public EnemyCurrentTask EnemyTask;
    public CharacterState EnemyState;


    private void Start()
    {
        Controller = GetComponent<CharacterController>();

        EnemyAction = new EnemyCurrentAction();
        EnemyState = new CharacterState();
        EnemyTask = new EnemyCurrentTask();

        _trampolineMask = LayerMask.GetMask("Trampoline");
    }

    private void FixedUpdate()
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
        Vector3 searchPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + _searchRadius + 2);
        Collider[] hitInfo = Physics.OverlapSphere(searchPosition, _searchRadius, _trampolineMask);

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
                _destination = FindTrampoline();
                EnemyTask = EnemyCurrentTask.RunToTrampoline;
                break;
            case EnemyCurrentTask.RunToTrampoline:
                // Ничего не происходит, т.к. противник уже выполнил FindTrampoline() и начал двигаться к нему.
                // Так нужно, чтобы игрок каждый раз не искал заново батут, а лишь продолжал двигаться к уже назначеной цели
                break;
            case EnemyCurrentTask.RunForward:
                _destination = new Vector3(enemyPosition.x, enemyPosition.y, enemyPosition.z + _moveForwardDistance);
                break;
        }

        if (Agent.enabled == true) { Agent.SetDestination(_destination); }
    }

    protected override void Run()
    {
        Vector3 moveHorizontal = transform.forward * Agent.speed;
        Controller.Move(moveHorizontal * Time.deltaTime);
    }

    protected override void TileJump()
    {
        Velocity.y = Mathf.Sqrt(JumpHeight * -2 * Gravity);
        Velocity.y += Gravity * Time.deltaTime;

        Controller.Move(Velocity * Time.deltaTime);

        EnemyAction = EnemyCurrentAction.Fall;
    }

    protected override void Fall()
    {
        Velocity.y += Gravity * Time.deltaTime;

        Controller.Move(Velocity * Time.deltaTime);
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
