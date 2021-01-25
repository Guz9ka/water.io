using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponController : MonoBehaviour
{
	public enum WeaponState
	{
		Idle,
		Firing,
		Reloading,
		Broken
	}

	public WeaponConfiguration Configuration;

	[Header("Точка, которая создаёт прожектайлы")]
	public Transform ProjectilePosition;

	/// <summary>
	/// Флаг, отвечающий за работоспособность компонента.
	/// </summary>
	public bool Deactivated = false;

	// Режим работы "Направлять на конкретную точку"
	public bool LookAt = false;
	// Точка
	public Transform LookAtTransform;

	/// <summary>
	/// Текущее состояние "оружия"
	/// </summary>
	public WeaponState State = WeaponState.Idle;

	private AudioSource audioSource = null;

	// Текущее кол-во патрон в магазине
	private int currentAmmoCount;

	// Направление стрельбы.
	private Ray shootRay;

	private void Start()
	{
		this.audioSource = this.GetComponent<AudioSource>();
		this.currentAmmoCount = this.Configuration.MagazineCapacity;
	}

	private void Update()
	{
		Debug.DrawRay(this.ProjectilePosition.position, this.ProjectilePosition.forward, Color.red);
		if (this.Deactivated)
		{
			return;
		}

		if (Input.GetButton("Reload"))
		{
			this.Reload();
		}

		if (Input.GetButton("Fire1"))
		{
			// Если у нас есть препятствие, то не можем стрелять
			if (Physics.Raycast(new Ray(this.ProjectilePosition.position, this.ProjectilePosition.forward), 10))
			{
				return;
			}

			if (this.State == WeaponState.Broken)
			{
				this.PlayAudio(this.Configuration.BrokenSound);
				return;
			}

			if (this.State != WeaponState.Firing && this.State != WeaponState.Reloading)
			{
				this.StartCoroutine(this.Fire());
			}
		}
	}

	private void LateUpdate()
	{
		if (this.LookAt && this.LookAtTransform != null)
		{
			this.transform.LookAt(this.LookAtTransform);
		}
	}

	private IEnumerator Reload()
	{
		this.State = WeaponState.Reloading;

		this.PlayAudio(this.Configuration.ReloadSound);

		yield return new WaitForSecondsRealtime(this.Configuration.ReloadSpeed);
		this.currentAmmoCount = this.Configuration.MagazineCapacity;

		this.State = WeaponState.Idle;
	}

	private IEnumerator Fire()
	{
		this.State = WeaponState.Firing;

		this.currentAmmoCount--;



		this.PlayAudio(this.Configuration.FireSound);
		GameObject projectile = GameObject.Instantiate(this.Configuration.ProjectilePrefab);
		projectile.transform.position = this.ProjectilePosition.position;
		Rigidbody rb = projectile.GetComponent<Rigidbody>();
		rb.AddForce(ProjectilePosition.forward * this.Configuration.ProjectileSpeed, ForceMode.Impulse);

		yield return new WaitForSecondsRealtime(this.Configuration.FireRate);

		if (this.currentAmmoCount <= 0)
		{
			this.StartCoroutine(this.Reload());
		}

		if (this.State != WeaponState.Reloading)
		{
			this.State = WeaponState.Idle;
		}
	}

	/// <summary>
	/// Просто сахарок, дабы не рисовать каждый раз проверки
	/// </summary>
	private void PlayAudio(AudioClip clip)
	{
		if (clip != null)
		{
			this.audioSource.PlayOneShot(clip);
		}
	}
}
