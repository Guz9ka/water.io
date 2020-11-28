using UnityEngine;

public class Trampoline : MonoBehaviour, ITrampoline
{
    private Player _player;
    private Enemy _enemy;
    private Character _char;

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

            _player.Velocity.y = Mathf.Sqrt(trampolineJumpForce * -2 * _player.Gravity);
            _player.Velocity.y += _player.Gravity * Time.deltaTime;

            _player.Controller.Move(_player.Velocity * Time.deltaTime);

            _player.PlayerAction = PlayerCurrentAction.Fall;
        }
    }

    public void JumpOnTrampolineEnemy()
    {
        if(_enemy != null)
        {
            //bug
            _enemy.Agent.enabled = false;

            _enemy.Velocity.y = Mathf.Sqrt(trampolineJumpForce * -2 * _enemy.Gravity);
            _enemy.Velocity.y += _enemy.Gravity * Time.deltaTime;

            _enemy.Controller.Move(_enemy.Velocity * Time.deltaTime);
            
            _enemy.EnemyAction = EnemyCurrentAction.Fall;
        }
    }
}
