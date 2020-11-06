using System.Collections;
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

    public float playerSpeedOriginal; //изначальный параметр скорости, к которому потом "скорость в данный момент" может вернуться
    public float playerMoveSpeed; //скорость в данный момент
    public float playerSlideSpeed;
    public float playerJumpHeight;

    public float moveAfterWin;

    [Header("Рассчет физики")]
    public float gravity = -9.81f;
    public Vector3 velocity;

    protected virtual void Move() { }
    protected virtual void CheckPlayerState() { }
    protected virtual void Run() { }
    protected virtual void Fall() { }
    protected virtual void Jump() { }
    protected virtual void PlayerDied() { }
    protected virtual void PlayerRevived() { }
    protected virtual void PlayerWon() { }
    public virtual void SlideOnSlide(Vector3 playerRotation) { }
    public virtual void JumpOnTrampoline(float jumpForce) { }
}
