using UnityEngine;

[CreateAssetMenu(fileName = "WeaponProjectileConfiguration", menuName = "ScriptableObjects/Projectile", order = 1)]
public class ProjectileConfiguration : ScriptableObject
{
	public bool DestroyOnCollide;
	public float DestroyDelay;

	[Header("от массы зависит дамаг.")]
	public float Mass;

	public bool CanExplode;
	public float ExplosionRate;
	public float ExplosionDelay;
	public float ExplosionRadius;
}
