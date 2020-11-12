using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisionReactor : MonoBehaviour
{
    private Enemy enemyBehaviour;

    [Header("Переменные объектов")]
    public float TrampolineJumpForce;
    public Vector3 SlideRotation;

    void Start()
    {
        enemyBehaviour = gameObject.GetComponent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Obstacle":
                enemyBehaviour.TriggerDeathEvent();
                break;
            case "Coin":
                Destroy(other.gameObject);
                break;
            case "Trampoline":
                enemyBehaviour.JumpOnTrampoline(TrampolineJumpForce);
                break;
            case "Slide":
                enemyBehaviour.SlideOnSlide(SlideRotation);
                break;
            case "End Zone":
                enemyBehaviour.TriggerWinEvent();
                break;
        }
    }
}
