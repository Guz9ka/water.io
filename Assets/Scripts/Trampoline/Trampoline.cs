using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour, ITrampoline
{
    private Player _player;

    [Header("Параметры батута")]
    [SerializeField]
    private float trampolineJumpForce;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UseTrampoline(other.gameObject);
        }
    }

    void UseTrampoline(GameObject playerObject)
    {
        _player = playerObject.GetComponent<Player>();
        JumpOnTrampoline();
    }

    public void JumpOnTrampoline()
    {
        if(_player != null)
        {
            _player.PlayerAction = PlayerCurrentAction.JumpOnTrampoline;

            _player.Velocity.y = Mathf.Sqrt(trampolineJumpForce * -2 * _player.gravity);
            _player.Velocity.y += _player.gravity * Time.deltaTime;

            _player.Controller.Move(_player.Velocity * Time.deltaTime);

            _player.PlayerAction = PlayerCurrentAction.Fall;
        }
    }
}
