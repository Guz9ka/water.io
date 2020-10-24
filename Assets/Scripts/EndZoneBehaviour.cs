using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndZoneBehaviour : MonoBehaviour
{
    public float MoveAfterWin;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            PlayerBehaviour playerBeh = other.gameObject.GetComponent<PlayerBehaviour>();
            playerBeh.moveAfterWin = MoveAfterWin;
            playerBeh.TriggerWinEvent();
        }
    }
}
