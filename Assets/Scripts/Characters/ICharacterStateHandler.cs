using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterStateHandler
{
    void GroundCheck();
    void TileJumpCheck();
    IEnumerator TileJumpSwitch();

    void TriggerDeathEvent();
    void TriggerReviveEvent();
    void TriggerWinEvent();

    void CharacterDied();
    void CharacterRevived();
    void CharacterWon();

    void OnGameStart();
    void OnGameEnd();
}
