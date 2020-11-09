using UnityEngine;

public class PlayerHeadCollisionsReactor : MonoBehaviour
{
    private PlayerMovement playerBehaviour;

    void Start()
    {
        playerBehaviour = GetComponentInParent<PlayerMovement>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Second Tier")
        {
            playerBehaviour.TriggerDeathEvent();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            playerBehaviour.TriggerDeathEvent();
        }
    }
}
