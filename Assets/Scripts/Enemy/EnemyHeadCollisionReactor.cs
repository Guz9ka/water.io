using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHeadCollisionReactor : MonoBehaviour
{
    private EnemyBehaviour enemyBehaviour;

    void Start()
    {
        enemyBehaviour = GetComponentInParent<EnemyBehaviour>();
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
