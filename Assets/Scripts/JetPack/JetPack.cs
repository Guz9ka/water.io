using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour, IJetPack
{
    private Player _player;

    [Header("Параметры джетпака")]
    [SerializeField]
    private float _flyHeight;
    [SerializeField]
    private float _flyUpDuration;
    [SerializeField]
    private float _flyDuration;

    [SerializeField]
    float _flyJoystickSensetivity;
    [SerializeField]
    float _playerFlySpeed;
    bool _flyingNow = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerJetpackUse(other.gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (_flyingNow && _player != null)
        {
            switch (_player.PlayerAction)
            {
                case PlayerCurrentAction.FlyingUp:
                    FlyUp();
                    break;
                case PlayerCurrentAction.FlyingForward:
                    FlyForward();
                    break;
            }
        }
    }

    public void TriggerJetpackUse(GameObject playerObject)
    {
        if (!_flyingNow)
        {
            _player = playerObject.GetComponent<Player>();
            StartCoroutine(UseJetpack());
        }
    }

    public IEnumerator UseJetpack()
    {
        _flyingNow = true;

        _player.PlayerAction = PlayerCurrentAction.FlyingUp;
        _player.gravity = -1f;

        yield return new WaitForSeconds(_flyUpDuration);

        _player.PlayerAction = PlayerCurrentAction.FlyingForward;
        _player.gravity = -9.8f;

        yield return new WaitForSeconds(_flyDuration);

        _player.PlayerAction = PlayerCurrentAction.Fall;
        _flyingNow = false;
    }

    public void FlyUp()
    {
        _player.Velocity.y = Mathf.Sqrt(_flyHeight * -2 * _player.gravity);
        _player.Velocity.y += _player.gravity * Time.deltaTime;

        _player.Controller.Move(_player.Velocity * Time.deltaTime);
    }

    public void FlyForward()
    {
        float moveSides = _player.Joystick.Horizontal * _flyJoystickSensetivity;
        Vector3 moveHorizontal = transform.forward * _playerFlySpeed + transform.right * moveSides;

        _player.Controller.Move(moveHorizontal * Time.deltaTime);
    }

}
