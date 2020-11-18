using UnityEngine;

public class Trampoline : MonoBehaviour, ITrampoline
{
    private Player _player;
    private Enemy _enemy;

    [Header("Параметры батута")]
    [SerializeField]
    private float trampolineJumpForce;

    private void OnTriggerEnter(Collider other)
    {
        bool isCharacter = other.CompareTag("Player") || other.CompareTag("Enemy");

        if (isCharacter)
        {
            UseTrampoline(other.gameObject);
        }
    }

    void UseTrampoline(GameObject character)
    {
        if(character.GetComponent<Player>() != null)
        {
            _player = character.GetComponent<Player>();
            JumpOnTrampolinePlayer();
        }
        else if(character.GetComponent<Enemy>() != null)
        {
            _enemy = character.GetComponent<Enemy>();
            JumpOnTrampolineEnemy();
        }
    }

    public void JumpOnTrampolinePlayer()
    {
        if (_player != null)
        {
            _player.PlayerAction = PlayerCurrentAction.JumpOnTrampoline;

            _player.Velocity.y = Mathf.Sqrt(trampolineJumpForce * -2 * _player.gravity);
            _player.Velocity.y += _player.gravity * Time.deltaTime;

            _player.Controller.Move(_player.Velocity * Time.deltaTime);

            _player.PlayerAction = PlayerCurrentAction.Fall;
        }
    }

    public void JumpOnTrampolineEnemy()
    {
        if(_enemy != null)
        {
            _enemy.Agent.enabled = false;

            _enemy.Velocity.y = Mathf.Sqrt(trampolineJumpForce * -2 * _enemy.gravity);
            _enemy.Velocity.y += _enemy.gravity * Time.deltaTime;

            _enemy.controller.Move(_enemy.Velocity * Time.deltaTime);

            _enemy.EnemyAction = EnemyCurrentAction.Fall;
        }
    }
}
