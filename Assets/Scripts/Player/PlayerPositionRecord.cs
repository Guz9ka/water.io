using UnityEngine;

[System.Serializable]
public class PlayerPositionRecord
{
    public string tag;

    public bool writeAvailable = true;
    public Vector3 lastWritedPosition;
    public float rewriteDelay;
}
