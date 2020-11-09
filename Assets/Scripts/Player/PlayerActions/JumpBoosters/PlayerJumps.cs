using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumps : MonoBehaviour, IJumpable
{
    private PlayerMovement playerActions;

    [Header("Параметры батута")]
    [SerializeField]
    private float trampolineJumpForce;

    [Header("Параметры ботинков для прыжка")]
    [SerializeField]
    private float bootsJumpForce;
    [SerializeField]
    private float jumpBootsForwardSpeed;

    void Start()
    {
        playerActions = GetComponent<PlayerMovement>();
    }

    public void JumpOnTrampoline() //триггерится через батут
    {
        playerActions.velocity.y = Mathf.Sqrt(trampolineJumpForce * -2 * playerActions.gravity);
        playerActions.velocity.y += playerActions.gravity * Time.deltaTime;

        playerActions.controller.Move(playerActions.velocity * Time.deltaTime);

        playerActions.PlayerAction = PlayerCurrentAction.Fall;
    }

    public void JumpOnBoots()
    {
        playerActions.velocity.y = Mathf.Sqrt(bootsJumpForce * -2 * playerActions.gravity);
        playerActions.velocity.y += playerActions.gravity * Time.deltaTime;
        playerActions.playerMoveSpeed = jumpBootsForwardSpeed;

        playerActions.controller.Move(playerActions.velocity * Time.deltaTime);

        playerActions.PlayerAction = PlayerCurrentAction.Fall;
    }
}
