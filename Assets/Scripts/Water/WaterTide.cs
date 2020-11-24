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
    private WaterState _waterState;
    [SerializeField] 
    private GameObject _water;

    private Tide _currentTide; 
    [SerializeField] 
    private List<Tide> _tides;
    [SerializeField] 
    private float _tideSpeed;

    //[Header("Событие прилива")]
    private delegate void TideStart();
    private event TideStart OnTideRaising;

    private void Start()
    {
        _currentTide = _tides[0];

        _waterState = new WaterState();

        OnTideRaising += StartTide;
    }

    private void Update()
    {
        TryStartTide();
    }

    private void TryStartTide()
    {
        if (_tides.Count > _tides.IndexOf(_currentTide) + 1)
        {
            if (SceneTime.singleton.TimeCurrent > _currentTide.Time && _waterState == WaterState.Static)
            {
                OnTideRaising.Invoke();
            }
        }
    }

    private void StartTide()
    {
        Vector3 waterPosition = _water.transform.position;
        Vector3 waterNewPosition = new Vector3(waterPosition.x, waterPosition.y + _currentTide.Height, waterPosition.z);

        float moveDuration = _tides[_tides.IndexOf(_currentTide) + 1].Time - _currentTide.Time;
        _water.transform.DOMoveY(waterNewPosition.y, moveDuration);

        SetNextTide();
    }

    private void SetNextTide()
    {
        _waterState = WaterState.Static;
        _currentTide = _tides[_tides.IndexOf(_currentTide) + 1];
    }

}
