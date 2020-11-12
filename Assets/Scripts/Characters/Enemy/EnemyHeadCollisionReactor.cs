using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadCollisionReactor : MonoBehaviour
{
    private Enemy enemyBehaviour;

    void Start()
    {
        enemyBehaviour = GetComponentInParent<Enemy>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Second Tier")
        {
            enemyBehaviour.TriggerDeathEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            enemyBehaviour.TriggerDeathEvent();
        }
    }
}
