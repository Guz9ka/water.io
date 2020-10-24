using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionsReactor : MonoBehaviour
{
    private PlayerBehaviour playerBehaviour;

    [Header("Переменные объектов")]
    public float TrampolineJumpForce;
    public Vector3 SlideRotation;

    void Start()
    {
        playerBehaviour = gameObject.GetComponent<PlayerBehaviour>();
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
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
