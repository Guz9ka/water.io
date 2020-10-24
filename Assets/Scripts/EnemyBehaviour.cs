using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class EnemyBehaviour : Player
{
    public PlayerCurrentAction enemyAction;
    private GameObject enemy;

    [Header("Смена состояний врага")]
    public CharacterController controller;

    [Header("Проверка наличия земли под врагом")]
    public Transform groundChecker;
    public float groudCheckDistance;
    private LayerMask groundMask;

    [Header("Смена состояний врага")]
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
        enemyAction = new PlayerCurrentAction();
    }

    void FixedUpdate()
    {

    }

    protected override void Move()
    {
        Run();

        switch (enemyAction)
        {
            case PlayerCurrentAction.Run:
                //игрок всегда бежит вперед
                break;
            case PlayerCurrentAction.Fall:
                Fall();
                break;
            case PlayerCurrentAction.Jump:
                Jump();
                break;
            case PlayerCurrentAction.Crawl:
                Crawl();
                break;
            case PlayerCurrentAction.Slide:
                //выполняется через скрипт горки
                break;
        }
    }

    #region Действия противника
    protected override void Jump()
    {
        velocity.y = Mathf.Sqrt(playerJumpHeight * -2 * gravity);
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        enemyAction = PlayerCurrentAction.Fall;
    }

    protected override void Crawl()
    {
        enemyAction = PlayerCurrentAction.Crawl;
        controller.height = 1.2f;
        //задержка

        enemyAction = PlayerCurrentAction.Run;
    }

    protected override void Run()
    {
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

        enemyAction = PlayerCurrentAction.Fall;
    }

    public override void SlideOnSlide(Vector3 PlayerRotation)
    {
        enemyAction = PlayerCurrentAction.Slide;

        enemy.transform.rotation = Quaternion.Euler(PlayerRotation);

        //добавить визуальный поворот скина
    }
    #endregion

    void GroundCheck()
    {
        bool isGrounded = Physics.CheckSphere(groundChecker.position, groudCheckDistance, groundMask);

        if (isGrounded == false && playerAction != PlayerCurrentAction.Jump)
        {
            enemyAction = PlayerCurrentAction.Fall;
        }
        else if (isGrounded == true && playerAction == PlayerCurrentAction.Jump || playerAction == PlayerCurrentAction.Fall)
        {
            enemyAction = PlayerCurrentAction.Run;
        }
    }

    #region Триггеры смены состояний игрока
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
        playerState = PlayerState.Dead;

        ResetCharacteristics();

        player.transform.position = player.GetComponent<PositionRestore>().RestorePosition(DeathReason);
        DeathReason = null;

        Invoke("TriggerReviveEvent", reviveDelay);
    }

    protected void EnemyRevived()
    {
        playerState = PlayerState.Alive;

        ResetCharacteristics();
    }

    protected void EnemyWinned()
    {
        playerState = PlayerState.Win;

        controller.Move(new Vector3(0, 0, Mathf.Lerp(0, moveAfterWin, playerMoveSpeed * Time.deltaTime)));

        
    }
    #endregion

    private void ResetCharacteristics()
    {
        player.transform.rotation = Quaternion.Euler(Vector3.zero);
    }
}
