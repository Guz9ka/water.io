using UnityEngine;

public class JumpBoots : MonoBehaviour, IJumpBoots
{
    private Player _player;

    [Header("Параметры ботинков для прыжка")]
    [SerializeField]
    private float bootsJumpForce;
    [SerializeField]
    private float jumpBootsForwardSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UseJumpBoots(other.gameObject);
        }
    }

    public void UseJumpBoots(GameObject playerObject)
    {
        _player = playerObject.GetComponent<Player>();
        JumpOnBoots();
    }

    public void JumpOnBoots()
    {
        if (_player != null)
        {
            _player.PlayerAction = PlayerCurrentAction.JumpOnBoots;

            _player.Velocity.y = Mathf.Sqrt(bootsJumpForce * -2 * _player.gravity);
            _player.Velocity.y += _player.gravity * Time.deltaTime;
            _player.MoveSpeed = jumpBootsForwardSpeed;

            _player.controller.Move(_player.Velocity * Time.deltaTime);

            _player.PlayerAction = PlayerCurrentAction.Fall;
        }
    }
}
