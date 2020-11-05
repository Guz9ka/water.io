using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsReactor : MonoBehaviour //Собрал в отдельный скрипт чтобы было больше производительности
{
    private PlayerBehaviour playerBehaviour;

    [Header("Параметры батута")]
    [SerializeField]
    private float TrampolineJumpForce;

    [Header("Параметры горки")]
    [SerializeField]
    private Vector3 SlideRotation;

    [Header("Параметры джетпака")]
    [SerializeField]
    private float FlyHeight;
    [SerializeField]
    private float FlyUpDuration;
    [SerializeField]
    private float FlyDistance;
    [SerializeField]
    private float FlyDuration;

    void Start()
    {
        playerBehaviour = gameObject.GetComponent<PlayerBehaviour>();
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Jetpack":
                Destroy(other.gameObject);
                playerBehaviour.TriggerJetpackUse(FlyHeight, FlyUpDuration, FlyDistance, FlyDuration);
                break;
            case "Obstacle":
                playerBehaviour.TriggerDeathEvent("Obstacle");
                break;
            case "Coin":
                Destroy(other.gameObject);
                break;
            case "Trampoline":
                playerBehaviour.JumpOnTrampoline(TrampolineJumpForce);
                break;
            case "Slide":
                playerBehaviour.SlideOnSlide(SlideRotation);
                break;
            case "End Zone":
                playerBehaviour.TriggerWinEvent();
                break;
        }
    }

}
