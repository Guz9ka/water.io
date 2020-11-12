using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SceneTime : MonoBehaviour
{
    public static SceneTime singleton { get; private set; }

    [Header("Параметры таймера")]
    public int TimeCurrent;
    Timer timer;
    TimerCallback timerCallback;

    private void Awake()
    {
    }

    private void Start()
    {
        singleton = this;

        timerCallback = new TimerCallback(TimerCallback);
        timer = new Timer(timerCallback, null, 0, 1000);
    }

    void TimerCallback(object time)
    {
        TimeCurrent += 1;
    }
}
