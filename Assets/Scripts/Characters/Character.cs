using System.Collections;
using UnityEngine;

public class Character : MonoBehaviour
{
    [Header("Параметры игрока")]
    [SerializeField]
    protected GameObject ChatacterSkin;

    public float MoveSpeed; //скорость в данный момент
    public float OriginalSpeed; //изначальный параметр скорости, к которому потом "скорость в данный момент" может вернуться
    public float JumpHeight;

    [Header("Рассчет физики")]
    public float gravity;
    public Vector3 Velocity;

    protected virtual void Move() { }
    protected virtual void Run() { }
    protected virtual void Fall() { }
    protected virtual void TileJump() { }
}
