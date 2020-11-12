using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

enum PushDirection
{
    Left,
    Right
}

public class PlayerPush : MonoBehaviour
{
    private Vector3 playerPosition;

    [Header("Параметры заряда")]
    [SerializeField] 
    private float maxCharge; // макс. радиус
    [SerializeField] 
    private float chargeDuration; //длительность заряда
    private float currentCharge; //заряд в данный момент

    [Header("Параметры толчка")]
    private bool screenTouched = false;
    [SerializeField] 
    private float pushDistance; //дальность толчка 
    [SerializeField] 
    private float pushForce; //сила толчка

    private void Start()
    {
        AccumulateCharge();
        OnGameStart();
    }

    private void Update()
    {
        playerPosition = gameObject.transform.position;
        GetInput();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerPosition, currentCharge);
    }

    void GetInput()
    {
        if (screenTouched == false && Input.GetKey(KeyCode.Space)) { screenTouched = true; } //Input.touchCount > 0
        else if (screenTouched == true && !Input.GetKey(KeyCode.Space)) //Input.touchCount > 0
        {
            Debug.Log("Push!");
            screenTouched = false;
            GetEnemies();
            ResetCharge();
        }
    }

    void AccumulateCharge()
    {
        DOTween.To(() => currentCharge, x => currentCharge = x, maxCharge, chargeDuration);
    }

    void GetEnemies()
    {
        Collider[] detectedEnemies = Physics.OverlapSphere(playerPosition, currentCharge, LayerMask.GetMask("Enemy"));
        
        if (detectedEnemies != null)
        {
            foreach (var enemy in detectedEnemies)
            {
                PushDirection pushDirection = GetPushDirection(enemy.transform.position);
                DoPush(enemy.transform, pushDirection);
            }
        }
    }

    private PushDirection GetPushDirection(Vector3 enemyPosition)
    {
        if (enemyPosition.x < playerPosition.x) return PushDirection.Left;
        else return PushDirection.Right;
    }

    private void DoPush(Transform enemyTransform, PushDirection pushDirection)
    {
        switch (pushDirection)
        {
            case PushDirection.Left:
                enemyTransform.DOMoveX(enemyTransform.position.x - pushDistance, pushForce);
                break;
            case PushDirection.Right:
                enemyTransform.DOMoveX(enemyTransform.position.x + pushDistance, pushForce);
                break;
        }
    }

    void ResetCharge()
    {
        DOTween.RestartAll();
        currentCharge = 0;
        AccumulateCharge();
    }

    void OnGameStart()
    {
    }
}
