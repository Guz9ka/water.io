using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SceneTime : MonoBehaviour
{
    public static SceneTime singleton { get; private set; }

    [Header("Параметры таймера")]
    public int TimeCurrent;
    private Timer _timer;
    private TimerCallback _timerCallback;

    private void Awake()
    {
    }

    private void Start()
    {
        singleton = this;

        _timerCallback = new TimerCallback(TimerCallback);
        _timer = new Timer(_timerCallback, null, 0, 1000);
    }

    private void TimerCallback(object time)
    {
        TimeCurrent += 1;
    }
}
