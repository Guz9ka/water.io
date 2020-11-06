using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJetPack
{
    void TriggerJetpackUse(PlayerActions playerActions);
    //IEnumerator UseJetpack();
    void FlyUp();
    void FlyForward();
}
