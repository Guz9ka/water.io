using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRestore : MonoBehaviour
{
    GameObject player;

    public List<PlayerPositionRecord> PositionRecords;
    private Dictionary<string, PlayerPositionRecord> writedPositions;

    private void Start()
    {
        player = gameObject;

        writedPositions = new Dictionary<string, PlayerPositionRecord>();

        foreach (PlayerPositionRecord positionRecord in PositionRecords)
        {
            positionRecord.writeAvailable = true;
            writedPositions.Add(positionRecord.tag, positionRecord);
        }
    }

    private void Update()
    {
        CheckWriteState(); //тут проверяется нужно ли записать позицию, и, если нужно, то она записывается в память
    }

    #region Запись позиции
    void CheckWriteState()
    {
        //foreach (string Key in writedPositions.Keys)
        //{
        //    if (writedPositions[Key].writeAvailable)
        //    {
        //        StartCoroutine(WritePosition(Key));
        //    }
        //}
        if (writedPositions["Obstacle"].writeAvailable)
        {
            StartCoroutine(WritePosition("Obstacle"));
        }
    }

    IEnumerator WritePosition(string key)
    {
        writedPositions[key].writeAvailable = false;
        writedPositions[key].lastWritedPosition = player.transform.position;

        yield return new WaitForSeconds(writedPositions[key].rewriteDelay);
        writedPositions[key].writeAvailable = true;
    }
    #endregion

    #region Восстановление позиции
    public Vector3 RestorePosition(string tag)
    {
        Invoke("RestartPositionRecord", 0.1f);
        return writedPositions[tag].lastWritedPosition;
    }

    void RestartPositionRecord()
    {
        foreach (string Key in writedPositions.Keys)
        {
            StopAllCoroutines();
            StartCoroutine(WritePosition(Key));
        }
    }
    #endregion

    public void OnGameEnd()
    {
        StopAllCoroutines();
    }
}
