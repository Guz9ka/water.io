using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideBehaviour : MonoBehaviour
{
    public Vector3 SlideRotation;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("player on slide");
            PlayerBehaviour playerBeh = other.gameObject.GetComponent<PlayerBehaviour>();

            playerBeh.SlideOnSlide(SlideRotation);
        }
    }
}
