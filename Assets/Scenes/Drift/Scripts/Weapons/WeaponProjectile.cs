using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WeaponProjectile : MonoBehaviour
{
	public ProjectileConfiguration Configuration;

	//private Rigidbody rigidbody;

	private void Start()
	{
		//this.rigidbody = this.GetComponent<Rigidbody>();
	}

	private void Update()
	{
		if (!this.Configuration.DestroyOnCollide && this.Configuration.ExplosionDelay == 0)
		{
			this.StartCoroutine(this.DestroyProjectile());
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (this.Configuration.CanExplode)
		{
			this.StartCoroutine(this.Explode());
		}
		else
		{
			this.StartCoroutine(this.DestroyProjectile());
		}
	}

	private IEnumerator Explode()
	{
		yield return new WaitForSecondsRealtime(this.Configuration.ExplosionDelay);
		
		if (this.Configuration.DestroyOnCollide)
		{
			this.StartCoroutine(this.DestroyProjectile());
		}
	}

	private IEnumerator DestroyProjectile()
	{
		yield return new WaitForSecondsRealtime(this.Configuration.DestroyDelay);
		GameObject.Destroy(this.gameObject);
	}
}
