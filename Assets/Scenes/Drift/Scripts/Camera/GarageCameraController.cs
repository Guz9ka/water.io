using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageCameraController : MonoBehaviour
{
    private int currentPosition = 0;

    public Transform[] CameraPositions;
    public Camera Camera;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void GoNext()
	{
        if (this.currentPosition + 1 >= this.CameraPositions.Length)
		{
            this.currentPosition = -1;
		}
        this.currentPosition++;

        Transform t = this.CameraPositions[this.currentPosition];

        this.Camera.transform.position = t.position;
        this.Camera.transform.rotation = t.rotation;
	}

    public void GoBack()
	{
        if (this.currentPosition - 1 < 0)
        {
            this.currentPosition = 0;
        }
        this.currentPosition--;

        Transform t = this.CameraPositions[this.currentPosition];

        this.Camera.transform.position = t.position;
        this.Camera.transform.rotation = t.rotation;
    }
}
