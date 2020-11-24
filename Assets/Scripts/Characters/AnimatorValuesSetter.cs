using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorValuesSetter : MonoBehaviour
{
    private Character _character;
    [SerializeField]
    private Animator _animator;

    private void Start()
    {
        if(GetComponent<PlayerPush>() != null)
        {
            GetComponent<PlayerPush>().OnPlayerPushed += PlayerPushTrigger;
        }

        _character = GetComponent<Character>();
        _animator.applyRootMotion = false;
    }

    private void Update()
    {
        SetAnimatorValues();        
    }

    private void SetAnimatorValues()
    {
        _animator.SetFloat("velocityY", _character.Velocity.y);
        _animator.SetFloat("characterSpeed", _character.MoveSpeed);
    }

    private void PlayerPushTrigger()
    {
        _animator.SetTrigger("OnPlayerPushed");
    }
}
