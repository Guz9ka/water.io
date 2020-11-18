using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorValuesSetter : MonoBehaviour
{
    private Character character;
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        GetComponent<PlayerPush>().OnPlayerPushed += PlayerPushTrigger;

        character = GetComponent<Character>();
        animator.applyRootMotion = false;
    }

    private void Update()
    {
        SetAnimatorValues();        
    }

    private void SetAnimatorValues()
    {
        animator.SetFloat("velocityY", character.Velocity.y);
        animator.SetFloat("characterSpeed", character.MoveSpeed);
    }

    private void PlayerPushTrigger()
    {
        animator.SetTrigger("OnPlayerPushed");
    }
}
