using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterStateHandler
{
    void GroundCheck();
    void JumpCheck();
    void TriggerDeathEvent();
    void TriggerReviveEvent();
    void TriggerWinEvent();
    IEnumerator TileJumpSwitch();
    void PlayerDied();
    void PlayerRevived();
    void PlayerWon();
    void OnGameStart();
    void OnGameEnd();
}
