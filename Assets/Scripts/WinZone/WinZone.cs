using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool isCharacter = other.CompareTag("Player") || other.CompareTag("Enemy");

        if (isCharacter)
        {
            SetPlayerWin(other.gameObject);
        }
    }

    void SetPlayerWin(GameObject playerObject)
    {
        playerObject.GetComponent<ICharacterStateHandler>()?.TriggerWinEvent();
    }
}
