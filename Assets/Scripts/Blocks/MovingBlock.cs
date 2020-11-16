using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

enum DestinationSide
{
    Right,
    Left
}

public class MovingBlock : MonoBehaviour
{
    [Header("Состояния движения блока")]
    private float _targetPositionX;
    private DestinationSide _destinationSide;

    [Header("Параметры движения блока")]
    private float _startPositionX;
    [SerializeField]
    private float _moveDistance;
    [SerializeField]
    private float _moveDuration;

    private void Start()
    {
        _startPositionX = gameObject.transform.position.x;
        StartMoveRight();
    }

    private void Update()
    {
        CheckMoveEnd();
    }

    void StartMoveRight()
    {
        _destinationSide = DestinationSide.Right;

        _targetPositionX = _startPositionX + _moveDistance;
        gameObject.transform.DOMoveX(_targetPositionX, _moveDuration);
    }

    void StartMoveLeft()
    {
        _destinationSide = DestinationSide.Left;

        _targetPositionX = _startPositionX - _moveDistance;
        gameObject.transform.DOMoveX(_targetPositionX, _moveDuration);
    }

    void CheckMoveEnd()
    {
        if(gameObject.transform.position.x == _targetPositionX)
        {
            switch (_destinationSide)
            {
                case DestinationSide.Right:
                    StartMoveLeft();
                    break;
                case DestinationSide.Left:
                    StartMoveRight();
                    break;
            }
        }
    }
}
