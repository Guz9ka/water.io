using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsReactor : MonoBehaviour //Собрал в отдельный скрипт чтобы было больше производительности
{
    private PlayerMovement playerActions;

    [Header("Параметры горки")]
    [SerializeField]
    private Vector3 SlideRotation;

    void Start()
    {
        playerActions = gameObject.GetComponent<PlayerMovement>();
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Speed Booster":
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
                playerActions.jumpBooster.JumpOnTrampoline();
                break;
            case "Slide":
                playerActions.SlideOnSlide(SlideRotation);
                break;
            case "End Zone":
                playerActions.TriggerWinEvent();
                break;
            case "Jump Boots":
                playerActions.jumpBooster.JumpOnBoots();
                break;
        }
    }

}
