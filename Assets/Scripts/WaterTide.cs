using System.Collections.Generic;
using System.Threading;
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
        public int time;
        public float height;
    }

    public static WaterTide singleton { get; private set; }

    [Header("Параметры прилива")]
    private WaterState waterState;
    [SerializeField] private GameObject water;

    private Tide currentTide; 
    [SerializeField] public List<Tide> tides;
    [SerializeField] private float tideSpeed;

    private Vector3 waterNewPosition;

    //[Header("Событие прилива")]
    private delegate void TideStart();
    private event TideStart OnTideRaising;

    [Header("Таймер")]
    Timer timer;
    TimerCallback timerCallback;
    public int timeCurrent;

    private void Start()
    {
        singleton = this;
        currentTide = tides[0];

        waterState = new WaterState();

        timerCallback = new TimerCallback(TimerCallback);
        timer = new Timer(timerCallback, null, 0, 1000);

        OnTideRaising += StartTide;
    }

    private void Update()
    {
        CheckTideState();
        CheckTideTime();
    }

    private void CheckTideState()
    {
        switch (waterState)
        {
            case WaterState.Raising:
                WaterRaising();
                break;
        }
    }

    private void CheckTideTime()
    {
        if (tides.Count > tides.IndexOf(currentTide) + 1)
        {
            if (timeCurrent > currentTide.time && waterState == WaterState.Static)
            {
                OnTideRaising.Invoke();
            }
        }
    }

    private void StartTide()
    {
        if (tides.IndexOf(currentTide) != 0) { NextTide(); }
        Vector3 waterPosition = water.transform.position;
        waterNewPosition = new Vector3(waterPosition.x, waterPosition.y + currentTide.height, waterPosition.z);
        Debug.Log(waterNewPosition);
        waterState = WaterState.Raising;
    }


    void WaterRaising()
    {
        Vector3 waterPosition = water.transform.position;
        water.transform.position = Vector3.Lerp(waterPosition, waterNewPosition, tideSpeed * Time.deltaTime);
    }

    private void NextTide()
    {
        waterState = WaterState.Static;
        currentTide = tides[(tides.IndexOf(currentTide) + 1)];
    }

    void TimerCallback(object time)
    {
        timeCurrent += 1;
    }
}
