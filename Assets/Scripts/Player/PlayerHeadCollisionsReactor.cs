using UnityEngine;

public class PlayerHeadCollisionsReactor : MonoBehaviour
{
    private PlayerActions playerBehaviour;

    void Start()
    {
        playerBehaviour = GetComponentInParent<PlayerActions>();
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
