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

    [SerializeField] private float rearWheelsRotationScale;

    private float _inputVertical;
    private float _inputHorizontal;

    [SerializeField] private float breakingForce;
    [SerializeField] private float breakingWheelRotation;
    private bool _isBraking;

    private Rigidbody carRigidbody;

    private void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        GetInput();
        AddTorque();
        RotateWheels();
        ResetTorque();
        Brake();
    }

    private void ResetTorque()
    {
        if (carRigidbody.velocity.z < 0.2 && carRigidbody.velocity.z > -0.2 && _inputVertical < 0.2 && _inputVertical > 0.2)
        {
            wheelRR.motorTorque = 0;
            wheelRL.motorTorque = 0;
        }
    }

    private void GetInput()
    {
        _inputVertical = Input.GetAxis(axisName: "Vertical");
        _inputHorizontal = Input.GetAxis(axisName: "Horizontal");

        _isBraking = Input.GetKey(KeyCode.Space);
        Debug.Log(_isBraking);
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
        
        //wheelRR.steerAngle = maxWheelTurnAngle * _inputHorizontal / rearWheelsRotationScale * -1;
        //wheelRL.steerAngle = maxWheelTurnAngle * _inputHorizontal / rearWheelsRotationScale * -1;

    }

    private void Brake()
    {
        wheelFR.brakeTorque = _isBraking ? breakingForce : 0;
        wheelFL.brakeTorque = _isBraking ? breakingForce : 0;

        //wheelFR.sidewaysFriction.
    }
}
