using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private Transform _camera;
    [SerializeField]
    private Vector3 _offset;
    private Quaternion _startRotation;

    private void Start()
    {
        _startRotation = _camera.rotation;
    }

    private void Update()
    {
        _camera.localPosition = _offset;
        _camera.rotation = _startRotation;
    }
}
