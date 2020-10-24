using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampolineBehaviour : MonoBehaviour
{
    public float jumpForce;

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("player on trampoline");
            GameObject player = other.gameObject;
            PlayerBehaviour playerBehaviour = player.GetComponent<PlayerBehaviour>();
            playerBehaviour.JumpOnTrampoline(0);
            playerBehaviour.JumpOnTrampoline(jumpForce);
        }

        if (other.tag == "Enemy")
        {
            GameObject enemy = other.gameObject;
            //enemy.GetComponent<EnemyBehaviour>().JumpOnTrampoline(jumpForce);
        }
    }
}
