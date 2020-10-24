using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehaviour : Obstacle
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("player hit the obstacle");
            GameObject player = other.gameObject;
            player.GetComponent<PlayerBehaviour>().TriggerDeathEvent("Obstacle");
        }
        if (other.tag == "Enemy")
        {
            GameObject enemy = other.gameObject;
            //enemy.GetComponent<EnemyBehaviour>().SlideOnSlide();
        }
    }
}
