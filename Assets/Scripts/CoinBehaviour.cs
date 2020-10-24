using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinBehaviour : MonoBehaviour
{
    public float rotationSpeed;

    void FixedUpdate()
    {
        Rotate();
    }

    private void Rotate()
    {
        gameObject.transform.rotation = Quaternion.Euler(0, gameObject.transform.rotation.y * rotationSpeed, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
