using UnityEngine;

public class MouseAimCamera : MonoBehaviour
{
	public GameObject target;
	public float rotateSpeed = 5;
	Vector3 offset;

	void Start()
	{
		this.offset = this.target.transform.position - this.transform.position;
	}

	void LateUpdate()
	{
		float horizontal = Input.GetAxis("Mouse X") * this.rotateSpeed;

		this.transform.position += Vector3.right + new Vector3(horizontal, 0, 0);

		this.transform.LookAt(this.target.transform);
	}
}
