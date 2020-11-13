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
    private float targetPositionX;
    private DestinationSide destinationSide;

    [Header("Параметры движения блока")]
    private float startPositionX;
    [SerializeField]
    private float moveDistance;
    [SerializeField]
    private float moveDuration;

    private void Start()
    {
        startPositionX = gameObject.transform.position.x;
        StartMoveRight();
    }

    private void Update()
    {
        CheckMoveEnd();
    }

    void StartMoveRight()
    {
        destinationSide = DestinationSide.Right;

        targetPositionX = startPositionX + moveDistance;
        gameObject.transform.DOMoveX(targetPositionX, moveDuration);
    }

    void StartMoveLeft()
    {
        destinationSide = DestinationSide.Left;

        targetPositionX = startPositionX - moveDistance;
        gameObject.transform.DOMoveX(targetPositionX, moveDuration);
    }

    void CheckMoveEnd()
    {
        if(gameObject.transform.position.x == targetPositionX)
        {
            switch (destinationSide)
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
