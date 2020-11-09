using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeedBooster
{
    void TriggerSpeedBoosterUse(PlayerMovement playerActions);
    IEnumerator SpeedBoosterUse();
}
