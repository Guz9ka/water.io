using UnityEngine;

public class PlayerHeadCollisionsReactor : MonoBehaviour
{
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        playerBehaviour = GetComponentInParent<PlayerBehaviour>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Second Tier")
        {
            playerBehaviour.TriggerDeathEvent("Obstacle");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            playerBehaviour.TriggerDeathEvent("Water");
        }
    }
}
