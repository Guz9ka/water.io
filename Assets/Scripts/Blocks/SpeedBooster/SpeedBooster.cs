using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : MonoBehaviour, ISpeedBooster
{
    private Player _player;

    private bool speedBoosterUseAvailable = true;
    [SerializeField]
    private float _playerSpeed;
    [SerializeField]
    private float _speedBoostDuration;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryTriggerSpeedBoosterUse(other.gameObject);
        }
    }

    public void TryTriggerSpeedBoosterUse(GameObject playerObject)
    {
        if (speedBoosterUseAvailable)
        {
            _player = playerObject.GetComponent<Player>();
            StartCoroutine(UseSpeedBooster());
        }
    }

    public IEnumerator UseSpeedBooster()
    {
        StartUse();

        yield return new WaitForSeconds(_speedBoostDuration);

        EndUse();
    }

    private void StartUse()
    {
        speedBoosterUseAvailable = false;
        _player.MoveSpeed = _playerSpeed;
    }

    private void EndUse()
    {
        speedBoosterUseAvailable = true;
        _player.MoveSpeed = _player.MoveSpeedDefault;
    }
}
