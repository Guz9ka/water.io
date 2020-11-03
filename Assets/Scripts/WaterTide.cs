using DG.Tweening;
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
        CheckTideTime();
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
        Vector3 waterPosition = water.transform.position;
        Vector3 waterNewPosition = new Vector3(waterPosition.x, waterPosition.y + currentTide.height, waterPosition.z);

        float moveDuration = tides[(tides.IndexOf(currentTide) + 1)].time - currentTide.time;
        water.transform.DOMoveY(waterNewPosition.y, moveDuration);

        NextTide();
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
