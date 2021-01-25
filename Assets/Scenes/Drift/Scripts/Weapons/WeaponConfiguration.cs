using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "WeaponConfiguration", menuName = "ScriptableObjects/Weapon", order = 1)]
public class WeaponConfiguration : ScriptableObject
{
	public string Name;
	public decimal Cost;
	public Sprite Image;

	public float ProjectileSpeed;
	public GameObject ProjectilePrefab;

	[Space(10f)]
	public int MagazineCapacity;
	public float ReloadSpeed;
	public float FireRate;

	[Space(10f)]
	public float YSpeed;
	public float XSpeed;

	[Header("Audio")]
	public AudioClip NoAmmoSound;
	public AudioClip FireSound;
	public AudioClip ReloadSound;
	public AudioClip BrokenSound;
}
