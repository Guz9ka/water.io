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
    [SerializeField]
    private Transform _pushRangeVisual;
    [SerializeField]
    private Player _player;

    [Header("Параметры заряда")]
    [SerializeField] 
    private float _maxCharge; // макс. радиус
    [SerializeField] 
    private float _chargeDuration; //длительность заряда
    private float _currentCharge; //заряд в данный момент

    [Header("Параметры толчка")]
    private bool _screenTouched = false;
    [SerializeField] 
    private float _pushDistance; //дальность толчка 
    [SerializeField] 
    private float _pushForce; //сила толчка

    //[Header("Событие толчка игрока")]
    public delegate void PlayerPushed();
    public event PlayerPushed OnPlayerPushed;

    private void Start()
    {
        _player = GetComponent<Player>();
        AccumulateCharge();
        OnGameStart();

        OnPlayerPushed += GetEnemies;
        OnPlayerPushed += ResetCharge;
    }

    private void Update()
    {
        GetInput();
        UpdateChargeVisual();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _currentCharge);
    }

    private void GetInput()
    {
        bool input = Input.GetKey(KeyCode.Space) || Input.touchCount > 0;
        bool playerGrounded = _player.PlayerAction == PlayerCurrentAction.Run;

        if (_screenTouched == false && input) 
        { 
            _screenTouched = true; 
        }
        else if (_screenTouched == true && !input && playerGrounded)
        {
            _screenTouched = false;
            OnPlayerPushed.Invoke();
        }
    }

    private void AccumulateCharge()
    {
        DOTween.To(() => _currentCharge, x => _currentCharge = x, _maxCharge, _chargeDuration);
    }

    private void GetEnemies()
    {
        Collider[] detectedEnemies = Physics.OverlapSphere(transform.position, _currentCharge, LayerMask.GetMask("Enemy"));
        
        if (detectedEnemies != null)
        {
            foreach (Collider enemy in detectedEnemies)
            {
                PushDirection pushDirection = GetPushDirection(enemy.transform.position);
                PushEnemies(enemy.transform, pushDirection);
            }
        }
    }

    private PushDirection GetPushDirection(Vector3 enemyPosition)
    {
        if (enemyPosition.x < transform.position.x) return PushDirection.Left;
        else return PushDirection.Right;
    }

    private void PushEnemies(Transform enemyTransform, PushDirection pushDirection)
    {
        switch (pushDirection)
        {
            case PushDirection.Left:
                enemyTransform.DOMoveX(enemyTransform.position.x - _pushDistance, _pushForce);
                break;
            case PushDirection.Right:
                enemyTransform.DOMoveX(enemyTransform.position.x + _pushDistance, _pushForce);
                break;
        }
    }

    private void ResetCharge()
    {
        DOTween.RestartAll();
        _currentCharge = 0;
        AccumulateCharge();
    }

    private void UpdateChargeVisual()
    {
        _pushRangeVisual.localScale = new Vector3(_currentCharge, _pushRangeVisual.localScale.y, _currentCharge);
    }

    private void OnGameStart()
    {
    }
}
