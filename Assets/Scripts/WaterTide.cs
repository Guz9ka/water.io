using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum WaterState
{
    Low,
    High,
    Raising,
    Lowering
}

public class WaterTide : MonoBehaviour
{
    public static WaterTide singleton { get; private set; }

    [Header("Параметры прилива")]
    public GameObject water;

    public static WaterState waterState;
    public float tideHeight;
    public float tideLow;
    public float tideSpeed;

    public int tideCurrentNumber;
    public int tideLowCurrentNumber;
    public List<int> tideTime;
    public List<int> tideLowTime;

    //[Header("Событие прилива")]
    public delegate void Tide();
    public event Tide OnTideRaising;

    public delegate void TideEnd();
    public event TideEnd OnTideLowering;

    [Header("Таймер")]
    Timer timer;
    TimerCallback timerCallback;
    public int timeCurrent;

    private void Start()
    {
        singleton = this;

        waterState = new WaterState();

        timerCallback = new TimerCallback(TimerCallback);
        timer = new Timer(timerCallback, null, 0, 1000);

        OnTideRaising += StartTide;
        OnTideLowering += EndTide;
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
            case WaterState.Lowering:
                WaterLowering();
                break;
        }
    }

    private void CheckTideTime()
    {
        if (tideTime.Count > tideCurrentNumber)
        {
            if (timeCurrent > tideTime[tideCurrentNumber] && waterState != WaterState.Raising)
            {
                OnTideRaising.Invoke();
            }
        }
        if (tideLowTime.Count > tideLowCurrentNumber)
        {
            if (timeCurrent > tideLowTime[tideLowCurrentNumber] && waterState != WaterState.Lowering)
            {
                OnTideLowering.Invoke();
            }
        }
    }

    private void StartTide()
    {
        tideCurrentNumber += 1;
        waterState = WaterState.Raising;
    }

    private void EndTide()
    {
        tideLowCurrentNumber += 1;
        waterState = WaterState.Lowering;
    }

    void WaterRaising()
    {
        Vector3 waterPosition = water.transform.position;
        Vector3 waterNewPosition = new Vector3(waterPosition.x, tideHeight, waterPosition.z);
        waterPosition = Vector3.Lerp(waterPosition, waterNewPosition, tideSpeed * Time.deltaTime);

        water.transform.position = waterPosition;
        
        if (water.transform.position.y >= tideHeight - 0.1f) { waterState = WaterState.High; }
    }

    void WaterLowering()
    {
        Vector3 waterPosition = water.transform.position;
        Vector3 waterNewPosition = new Vector3(waterPosition.x, tideLow, waterPosition.z);
        waterPosition = Vector3.Lerp(waterPosition, waterNewPosition, tideSpeed * Time.deltaTime);
        
        water.transform.position = waterPosition;

        if (water.transform.position.y <= tideLow + 0.1f) { waterState = WaterState.Low; }
    }

    void TimerCallback(object time)
    {
        timeCurrent += 1;
    }
}
