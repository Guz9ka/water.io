using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeedBooster
{
    void TriggerSpeedBoosterUse(PlayerActions playerActions);
    IEnumerator SpeedBoosterUse();
}
