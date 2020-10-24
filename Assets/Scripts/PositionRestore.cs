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
        CheckWriteState();
    }

    void CheckWriteState()
    {
        foreach (string Key in writedPositions.Keys)
        {
            if (writedPositions[Key].writeAvailable)
            {
                StartCoroutine(WritePosition(Key));
            }
        }
    }

    IEnumerator WritePosition(string key)
    {
        writedPositions[key].writeAvailable = false;
        writedPositions[key].lastWritedPosition = player.transform.position;
        
        yield return new WaitForSeconds(writedPositions[key].rewriteDelay);
        writedPositions[key].writeAvailable = true;
    }

    public Vector3 RestorePosition(string tag)
    {
        Invoke("WritePosition", 0.1f);
        return writedPositions[tag].lastWritedPosition;
    }

    void WritePosition()
    {
        foreach (string Key in writedPositions.Keys)
        {
            StopAllCoroutines();
            StartCoroutine(WritePosition(Key));
        }
    }

    public void OnGameEnd()
    {
        StopAllCoroutines();
    }
}
