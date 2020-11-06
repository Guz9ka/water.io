using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsReactor : MonoBehaviour //Собрал в отдельный скрипт чтобы было больше производительности
{
    private PlayerActions playerActions;

    [Header("Параметры батута")]
    [SerializeField]
    private float TrampolineJumpForce;

    [Header("Параметры горки")]
    [SerializeField]
    private Vector3 SlideRotation;

    void Start()
    {
        playerActions = gameObject.GetComponent<PlayerActions>();
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "SpeedBooster":
                playerActions.speedBooster.TriggerSpeedBoosterUse(playerActions);
                break;
            case "Jetpack":
                Destroy(other.gameObject);
                playerActions.jetPack.TriggerJetpackUse(playerActions);
                break;
            case "Obstacle":
                playerActions.TriggerDeathEvent();
                break;
            case "Coin":
                Destroy(other.gameObject);
                break;
            case "Trampoline":
                playerActions.JumpOnTrampoline(TrampolineJumpForce);
                break;
            case "Slide":
                playerActions.SlideOnSlide(SlideRotation);
                break;
            case "End Zone":
                playerActions.TriggerWinEvent();
                break;
        }
    }

}
