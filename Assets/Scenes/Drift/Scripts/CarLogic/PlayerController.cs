using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public bool Deactivated = false;
	[SerializeField]
	private CarConfiguration _carConfiguration;

	#region Intermediate
	private Rigidbody rigidBody;
	private Bounds groupCollider;
	private float distToGround;

	// The actual value to be used (modification of parameters)
	public float rotate { get; private set; }
	private float acceleration;
	private float gripX;
	private float gripZ;
	private float rotationVelocity;
	private float slip;     // The value used based on Slip curves
	private Vector3 centerOfMass;

	// For determining drag direction
	private float isRight = 1.0f;
	private float isForward = 1.0f;

	private bool isRotating = false;
	private bool isGrounded = true;

	// Control signals
	private float inThrottle = 0f;
	[HideInInspector] 
	public float inTurn = 0f;
	private bool inBoost = false;
	private bool inSlip = false;

	private Vector3 vel = new Vector3(0f, 0f, 0f);
	private Vector3 pvelocity = new Vector3(0f, 0f, 0f);
	#endregion



	private void Start()
	{
		this.rigidBody = this.GetComponent<Rigidbody>();

		this.groupCollider = GetBounds(this.gameObject);     // Get the full collider boundary of group
		this.distToGround = this.groupCollider.extents.y;    // Pivot to the outermost collider

		// Move the CenterOfMass to a fraction of colliders boundaries
		this.rigidBody.centerOfMass = Vector3.Scale(this.groupCollider.extents, this.centerOfMass);
	}

	private void Update()
	{
		Debug.DrawRay(this.transform.position, this.rigidBody.velocity / 2, Color.green);
	}

	private void FixedUpdate()
	{
		if (this.Deactivated)
		{
			return;
		}

		#region Situational Checks
		this.acceleration = this._carConfiguration.Acceleration;
		this.rotate = this._carConfiguration.Rotate;
		this.gripX = this._carConfiguration.GripX;
		this.gripZ = this._carConfiguration.GripZ;
		this.rotationVelocity = this._carConfiguration.RotationVelocity;
		this.rigidBody.angularDrag = this._carConfiguration.AngularDragG;

		// Adjustment in slope
		this.acceleration = this.acceleration * Mathf.Cos(this.transform.eulerAngles.x * Mathf.Deg2Rad);
		this.acceleration = this.acceleration > 0f ? this.acceleration : 0f;
		this.gripZ = this.gripZ * Mathf.Cos(this.transform.eulerAngles.x * Mathf.Deg2Rad);
		this.gripZ = this.gripZ > 0f ? this.gripZ : 0f;
		this.gripX = this.gripX * Mathf.Cos(this.transform.eulerAngles.z * Mathf.Deg2Rad);
		this.gripX = this.gripX > 0f ? this.gripX : 0f;

		// A short raycast to check below
		this.isGrounded = Physics.Raycast(this.transform.position, -this.transform.up, this.distToGround + 0.1f);
		if (!this.isGrounded)
		{
			this.rotate = 0f;
			this.acceleration = 0f;
			this.gripX = 0f;
			this.gripZ = 0f;
			this.rigidBody.angularDrag = this._carConfiguration.AngularDragA;
		}

		// Start turning only if there's velocity
		if (this.pvelocity.magnitude < this._carConfiguration.MinRotationSpeed)
		{
			this.rotate = 0f;
		}
		else
		{
			this.rotate = this.pvelocity.magnitude / this._carConfiguration.MaxRotationSpeed * this.rotate;
		}

		if (this.rotate > this._carConfiguration.Rotate) this.rotate = this._carConfiguration.Rotate;

		// Calculate grip based on sideway velocity in hysteresis curve
		if (!this.inSlip)
		{
			// Normal => slip
			this.slip = this._carConfiguration.SlipL.Evaluate(Mathf.Abs(this.pvelocity.x) / this._carConfiguration.SlipMod);
			if (this.slip == 1f) this.inSlip = true;
		}
		else
		{
			// Slip => Normal
			this.slip = this._carConfiguration.SlipU.Evaluate(Mathf.Abs(this.pvelocity.x) / this._carConfiguration.SlipMod);
			if (this.slip == 0f) this.inSlip = false;
		}

		this.rotate *= (1f - 0.3f * this.slip);   // Overall rotation, (body + vector)
		this.rotationVelocity *= (1f - this.slip);          // The vector modifier (just vector)

		/* Should be:
         * 1. Moving fast       : local forward, world forward.
         * 2. Swerve left       : instantly rotate left, local sideways, world forward.
         * 3. Wheels turn a little : small adjustments to the drifting arc.
         * 3. Wheels turn right : everything the same, traction still gone.
         * 4. Slowing down      : instantly rotate right, local forward, world left.
         * 
         * Update, solution: Hysteresis, gradual loss but snappy return.
         */

		#endregion

		#region Logics
		// Execute the commands
		this.InputKeyboard();
		this.Controller();   // pvel assigment in here
		#endregion

		#region Passives
		// Get the local-axis velocity after rotation
		this.vel = this.transform.InverseTransformDirection(this.rigidBody.velocity);

		// Rotate the velocity vector
		// vel = pvel => Transfer all (full grip)
		if (this.isRotating)
		{
			this.vel = this.vel * (1 - this.rotationVelocity) + this.pvelocity * this.rotationVelocity; // Partial transfer
																										//vel = vel.normalized * speed;
		}

		// Sideway grip
		this.isRight = this.vel.x > 0f ? 1f : -1f;
		this.vel.x -= this.isRight * this.gripX * Time.deltaTime;  // Accelerate in opposing direction
		if (this.vel.x * this.isRight < 0f) this.vel.x = 0f;       // Check if changed polarity

		// Straight grip
		this.isForward = this.vel.z > 0f ? 1f : -1f;
		this.vel.z -= this.isForward * this.gripZ * Time.deltaTime;
		if (this.vel.z * this.isForward < 0f) this.vel.z = 0f;

		// Top speed
		if (this.vel.z > this._carConfiguration.MaxSpeed) this.vel.z = this._carConfiguration.MaxSpeed;
		else if (this.vel.z < -this._carConfiguration.MaxSpeed) this.vel.z = -this._carConfiguration.MaxSpeed;

		this.rigidBody.velocity = this.transform.TransformDirection(this.vel);
		#endregion
	}

	#region Controllers
	// Get input values from keyboard
	private void InputKeyboard()
	{
		this.inThrottle = Input.GetAxisRaw("Throttle");
		this.inTurn = Input.GetAxisRaw("Sideways");
	}

	// Executing the queued inputs
	private void Controller()
	{
		if (this.inBoost) this.acceleration *= this._carConfiguration.BoostMultiplier; // Higher acceleration

		if (this.inThrottle > 0.5f || this.inThrottle < -0.5f)
		{
			this.rigidBody.velocity += this.transform.forward * this.inThrottle * this.acceleration * Time.deltaTime;
			this.gripZ = 0f;     // Remove straight grip if wheel is rotating
		}

		this.isRotating = false;

		// Get the local-axis velocity before new input (+x, +y, and +z = right, up, and forward)
		this.pvelocity = this.transform.InverseTransformDirection(this.rigidBody.velocity);

		// Turn statically
		if (this.inTurn > 0.5f || this.inTurn < -0.5f)
		{
			float direction = (this.pvelocity.z < 0) ? -1 : 1;    // To fix direction on reverse
			this.RotateGradConst(this.inTurn * direction);
		}
	}
	#endregion

	#region Rotation Methods
	/* Advised to not read eulerAngles, only write: https://answers.unity.com/questions/462073/
     * As it turns out, the problem isn't there. */

	/* As is: Conflict with physical Y-axis rotation, must be disabled.
     * Current methods:
     * 1. Prevent rotational input when there's angular velocity.
     * 2. Significantly increase angular drag while grounded.
     * 3. Result: rotation responding to environment, responsive input, & natural stumbling.
     */

	Vector3 drot = new Vector3(0f, 0f, 0f);

	private void RotateGradConst(float isCW)
	{
		// Delta = right(taget) - left(current)
		this.drot.y = isCW * this.rotate * Time.deltaTime;
		this.transform.rotation *= Quaternion.AngleAxis(this.drot.y, this.transform.up);
		this.isRotating = true;
	}
	#endregion

	#region Utilities

	// Get bound of a large 
	public static Bounds GetBounds(GameObject obj)
	{
		// Switch every collider to renderer for more accurate result
		Bounds bounds = new Bounds();
		Collider[] colliders = obj.GetComponentsInChildren<Collider>();

		if (colliders.Length > 0)
		{

			//Find first enabled renderer to start encapsulate from it
			foreach (Collider collider in colliders)
			{

				if (collider.enabled)
				{
					bounds = collider.bounds;
					break;
				}
			}

			//Encapsulate (grow bounds to include another) for all collider
			foreach (Collider collider in colliders)
			{
				if (collider.enabled)
				{
					bounds.Encapsulate(collider.bounds);
				}
			}
		}
		return bounds;
	}
	#endregion
}
