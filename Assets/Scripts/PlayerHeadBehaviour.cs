using UnityEngine;

public class PlayerHeadBehaviour : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Second Tier")
        {
            gameObject.GetComponentInParent<PlayerBehaviour>().TriggerDeathEvent("Obstacle");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
        {
            gameObject.GetComponentInParent<PlayerBehaviour>().TriggerDeathEvent("Water");
        }
    }
}
