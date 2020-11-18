using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWaterDraw : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CharacterHead"))
        {
            other.gameObject.GetComponentInParent<ICharacterStateHandler>()?.TriggerDeathEvent();
        }
    }
}
