using System.Collections;
using UnityEngine;

public enum CharacterState
{
    Alive,
    Dead,
    Win,
    Menus
}

public class Character : MonoBehaviour
{
    [Header("Параметры игрока")]
    [SerializeField]
    protected GameObject ChatacterSkin;

    [HideInInspector]
    public float MoveSpeed; //скорость в данный момент
    public float MoveSpeedDefault; //изначальный параметр скорости, к которому потом "скорость в данный момент" может вернуться
    public float JumpHeight;

    [Header("Рассчет физики")]
    public float gravity;
    public Vector3 Velocity;

    protected virtual void Move() { }
    protected virtual void Run() { }
    protected virtual void Fall() { }
    protected virtual void TileJump() { }
}
