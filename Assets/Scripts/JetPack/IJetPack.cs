using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJetPack
{
    void TriggerJetpackUse(GameObject playerObject);
    IEnumerator UseJetpack();
    void FlyUp();
    void FlyForward();
}
