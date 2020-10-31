using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointRecorder : MonoBehaviour
{
    public GameObject lastCheckPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckPoint")
        {
            lastCheckPoint = other.gameObject;
        }
    }

    public Vector3 GetLastCheckPoint()
    {
        return lastCheckPoint.transform.position;
    }
}
