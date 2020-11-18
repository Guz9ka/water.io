using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpeedBooster
{
    void TryTriggerSpeedBoosterUse(GameObject playerObject);
    IEnumerator UseSpeedBooster();
}
