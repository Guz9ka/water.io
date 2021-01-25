using System;
using UnityEngine;

public class FallOutOfMapDetector : MonoBehaviour
{
    private Vector3 spawnP;
    private Quaternion spawnR;

    private void Start()
    {
        // Store start location & rotation
        this.spawnP = this.transform.position;
        this.spawnR = this.transform.rotation;
    }

    private void Update()
    {
        // Reset to spawn if out of bounds
        if (this.transform.position.y < -10)
        {
            this.transform.position = this.spawnP;
            this.transform.rotation = this.spawnR;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            this.gameObject.transform.rotation = Quaternion.identity;
        }
    }
}
