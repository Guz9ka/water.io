using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public enum WaterState
{
    Static,
    Raising
}


public class WaterTide : MonoBehaviour
{
    [System.Serializable]
    public class Tide
    {
        public int Time;
        public float Height;
    }

    [Header("Параметры прилива")]
    private WaterState waterState;
    [SerializeField] 
    private GameObject water;

    private Tide currentTide; 
    [SerializeField] 
    private List<Tide> tides;
    [SerializeField] 
    private float tideSpeed;

    //[Header("Событие прилива")]
    private delegate void TideStart();
    private event TideStart OnTideRaising;

    private void Start()
    {
        currentTide = tides[0];

        waterState = new WaterState();

        OnTideRaising += StartTide;
    }

    private void Update()
    {
        TryStartTide();
    }

    private void TryStartTide()
    {
        if (tides.Count > tides.IndexOf(currentTide) + 1)
        {
            if (SceneTime.singleton.TimeCurrent > currentTide.Time && waterState == WaterState.Static)
            {
                OnTideRaising.Invoke();
            }
        }
    }

    private void StartTide()
    {
        Vector3 waterPosition = water.transform.position;
        Vector3 waterNewPosition = new Vector3(waterPosition.x, waterPosition.y + currentTide.Height, waterPosition.z);

        float moveDuration = tides[tides.IndexOf(currentTide) + 1].Time - currentTide.Time;
        water.transform.DOMoveY(waterNewPosition.y, moveDuration);

        SetNextTide();
    }

    private void SetNextTide()
    {
        waterState = WaterState.Static;
        currentTide = tides[tides.IndexOf(currentTide) + 1];
    }

}
