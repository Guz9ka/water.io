using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWaterDraw : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player Head"))
        {
            other.gameObject.GetComponent<PlayerStateHandler>()?.TriggerDeathEvent();
        }
        else if (other.CompareTag("Enemy Head"))
        {
            other.gameObject.GetComponent<Enemy>()?.TriggerDeathEvent();
        }
    }
}
