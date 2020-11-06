using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetPack : MonoBehaviour, IJetPack
{
    [Header("Параметры джетпака")]
    [SerializeField]
    private float flyHeight;
    [SerializeField]
    private float flyUpDuration;
    [SerializeField]
    private float flyDuration;

    [SerializeField]
    float flyJoystickSensetivity;
    [SerializeField]
    float playerFlySpeed;

    PlayerActions _playerActions;
    bool flyAvailable = true;

    public void TriggerJetpackUse(PlayerActions playerActions)
    {
        if (flyAvailable)
        {
            _playerActions = playerActions;
            StartCoroutine(UseJetpack());
        }
    }

    IEnumerator UseJetpack()
    {
        flyAvailable = false;

        _playerActions.PlayerAction = PlayerCurrentAction.FlyingUp;
        _playerActions.gravity = -1f;

        yield return new WaitForSeconds(flyUpDuration);

        _playerActions.gravity = -9.8f;
        _playerActions.PlayerAction = PlayerCurrentAction.FlyingForward;

        yield return new WaitForSeconds(flyDuration);

        flyAvailable = true;
        _playerActions.PlayerAction = PlayerCurrentAction.Fall;
    }

    public void FlyUp()
    {
        _playerActions.PlayerAction = PlayerCurrentAction.FlyingUp;

        _playerActions.velocity.y = Mathf.Sqrt(flyHeight * -2 * _playerActions.gravity);
        _playerActions.velocity.y += _playerActions.gravity * Time.deltaTime;

        _playerActions.controller.Move(_playerActions.velocity * Time.deltaTime);
    }

    public void FlyForward()
    {
        _playerActions.PlayerAction = PlayerCurrentAction.FlyingForward;

        float moveSides = _playerActions.joystick.Horizontal * flyJoystickSensetivity;
        Vector3 moveHorizontal = transform.forward * playerFlySpeed + transform.right * moveSides;

        _playerActions.controller.Move(moveHorizontal * Time.deltaTime);

    }

}
