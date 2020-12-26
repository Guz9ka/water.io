using System;
using UnityEngine;

public class CarSteeringAE86 : MonoBehaviour
{
    [Header("Wheel colliders")]
    [SerializeField] private WheelCollider wheelFR;
    [SerializeField] private WheelCollider wheelFL;
    [SerializeField] private WheelCollider wheelRR;
    [SerializeField] private WheelCollider wheelRL;

    [Space]
    [Header("Engine parametres")]
    [SerializeField] private int maxWheelTurnAngle;
    [SerializeField] private float maxTorque;
    [SerializeField] private float driftForce;

    private float _inputVertical;
    private float _inputHorizontal;

    private void Update()
    {
        GetInput();
        AddTorque();
        RotateWheels();
    }

    private void GetInput()
    {
        _inputVertical = Input.GetAxis(axisName:"Vertical");
        _inputHorizontal = Input.GetAxis(axisName: "Horizontal");
    }

    private void AddTorque()
    {
        wheelRR.motorTorque = maxTorque * _inputVertical * Time.deltaTime;
        wheelRL.motorTorque = maxTorque * _inputVertical * Time.deltaTime;
    }

    private void RotateWheels()
    {
        wheelFR.steerAngle = maxWheelTurnAngle * _inputHorizontal;
        wheelFL.steerAngle = maxWheelTurnAngle * _inputHorizontal;
    }
}
