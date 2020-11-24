using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBooster : MonoBehaviour, ISpeedBooster
{
    private Player _player;

    private bool _useAvailable = true;
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
        if (_useAvailable)
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
        _useAvailable = false;
        _player.MoveSpeed = _playerSpeed;
    }

    private void EndUse()
    {
        _useAvailable = true;
        _player.MoveSpeed = _player.MoveSpeedDefault;
    }
}
