using UnityEngine;

[CreateAssetMenu(fileName = "CarConfig", menuName = "ScriptableObjects/Car Configuration", order = 0)]
public class CarConfiguration : ScriptableObject
{
	public string Name;
	public int Cost;
	public Sprite Image;
	
	public GameObject CarPrefab;
	//Steering parameters
	public float Acceleration = 15.0f;         // In meters/second2
	public float BoostMultiplier = 4f / 3;          // In ratio
	public float MaxSpeed = 30.0f;      // In meters/second
	public float GripX = 12.0f;          // In meters/second2
	public float GripZ = 3.0f;          // In meters/second2
	public float Rotate = 190;       // In degree/second
	public float RotationVelocity = 0.8f;         // Ratio of forward velocity transfered on rotation
	
	public float MinRotationSpeed = 1f;           // Velocity to start rotating
	public float MaxRotationSpeed = 4f;           // Velocity to reach max rotation
	public AnimationCurve SlipL;    // Slip hysteresis static to full (x, input = speed)
	public AnimationCurve SlipU;    // Slip hysteresis full to static (y, output = slip ratio)
	public float SlipMod = 20f;     // Basically widens the slip curve
	
	// Ground & air angular drag
	// reduce stumbling time on ground but maintain on-air one
	public float AngularDragG = 5.0f;
	public float AngularDragA = 0.05f;
	
	// Center of mass, fraction of collider boundaries (= half of size)
	// 0 = center, and +/-1 = edge in the pos/neg direction.
	public Vector3 CenterOfMass = new Vector3(0f, .3f, 0f);
}
