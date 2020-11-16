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
    private Vector3 _playerPosition;
    [SerializeField]
    private Transform _pushRangeVisual;    

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

    private void Start()
    {
        AccumulateCharge();
        OnGameStart();
    }

    private void Update()
    {
        _playerPosition = gameObject.transform.position;
        GetInput();
        UpdateChargeVisual();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_playerPosition, _currentCharge);
    }

    void GetInput()
    {
        bool input = Input.GetKey(KeyCode.Space) || Input.touchCount > 0;

        if (_screenTouched == false && input) { _screenTouched = true; }
        else if (_screenTouched == true && !input)
        {
            Debug.Log("Push!");
            _screenTouched = false;
            GetEnemies();
            ResetCharge();
        }
    }

    void AccumulateCharge()
    {
        DOTween.To(() => _currentCharge, x => _currentCharge = x, _maxCharge, _chargeDuration);
    }

    void GetEnemies()
    {
        Collider[] detectedEnemies = Physics.OverlapSphere(_playerPosition, _currentCharge, LayerMask.GetMask("Enemy"));
        
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
        if (enemyPosition.x < _playerPosition.x) return PushDirection.Left;
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

    void ResetCharge()
    {
        DOTween.RestartAll();
        _currentCharge = 0;
        AccumulateCharge();
    }

    void UpdateChargeVisual()
    {
        _pushRangeVisual.localScale = new Vector3(_currentCharge, _pushRangeVisual.localScale.y, _currentCharge);
    }

    void OnGameStart()
    {
    }
}
