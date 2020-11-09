using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : MonoBehaviour, ISpeedBooster
{
    private bool speedBoosterUseAvailable = true;

    private PlayerMovement _playerActions;
    [SerializeField]
    private float _playerSpeed;
    [SerializeField]
    private float _speedBoostDuration;


    public void TriggerSpeedBoosterUse(PlayerMovement playerActions)
    {
        if (speedBoosterUseAvailable)
        {
            _playerActions = playerActions;
            StartCoroutine(SpeedBoosterUse());
        }
    }

    public IEnumerator SpeedBoosterUse()
    {
        StartUse();

        yield return new WaitForSeconds(_speedBoostDuration);

        EndUse();
    }

    private void StartUse()
    {
        speedBoosterUseAvailable = false;
        _playerActions.playerMoveSpeed = _playerSpeed;
    }

    private void EndUse()
    {
        speedBoosterUseAvailable = true;
        _playerActions.playerMoveSpeed = _playerActions.playerSpeedOriginal;
    }
}
