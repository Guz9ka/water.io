using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinRotation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.y * rotationSpeed, 0);
    }
}
