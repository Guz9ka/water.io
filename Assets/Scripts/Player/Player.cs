using UnityEngine;

public enum PlayerState
{
    Alive,
    Dead,
    Win,
    Menus
}

public class Player : MonoBehaviour
{
    [Header("Параметры игрока")]
    public GameObject playerSkin;

    protected string DeathReason;

    public float playerMoveSpeedOriginal; //изначальный параметр скорости, к которому потом "скорость в данный момент" может вернуться
    protected float playerMoveSpeed; //скорость в данный момент
    public float playerSlideSpeed;
    public float playerJumpHeight;

    //public float playerRotationSpeed;

    public float moveAfterWin;

    [Header("Рассчет физики")]
    protected const float gravity = -9.81f;
    protected Vector3 velocity;

    protected virtual void Move() { }
    protected virtual void CheckPlayerState() { }
    protected virtual void Run() { }
    protected virtual void Fall() { }
    protected virtual void Jump() { }
    protected virtual void Crawl() { }
    protected virtual void PlayerDied() { }
    protected virtual void PlayerRevived() { }
    protected virtual void PlayerWon() { }
    public virtual void SlideOnSlide(Vector3 playerRotation) { }
    public virtual void JumpOnTrampoline(float jumpForce) { }
}
