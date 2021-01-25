using UnityEngine;

public class CameraShake : MonoBehaviour
{
	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;

	// How long the object should shake for.
	public float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	void Awake()
	{
		if (this.camTransform == null)
		{
			this.camTransform = this.GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		this.originalPos = this.camTransform.localPosition;
	}

	void Update()
	{
		if (this.shakeDuration > 0)
		{
			this.camTransform.localPosition = this.originalPos + Random.insideUnitSphere * this.shakeAmount;

			this.shakeDuration -= Time.deltaTime * this.decreaseFactor;
		}
		else
		{
			this.shakeDuration = 0f;
			this.camTransform.localPosition = this.originalPos;
		}
	}
}
