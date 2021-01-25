using UnityEngine;

public class Wheel : MonoBehaviour
{
    [SerializeField] private float modifier = 0.1f;   //Ideally time.deltaTime

    private PlayerController car;
    private Vector3 initialRotation;

    private void Start() 
    {
        car = GetComponentInParent<PlayerController>();
        initialRotation = transform.localEulerAngles;  // Rotation relative to parent (car)
    }

    private void Update() 
    {
        // Rotate this according to the rotation input value
        float rotate = this.car.inTurn * this.car.rotate * modifier;
        transform.localEulerAngles = this.initialRotation + new Vector3(0f, rotate, 0f);
    }
}
