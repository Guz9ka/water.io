using System.Linq;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
	public enum ConfigurationType
	{
		Weapon,
		Vehicle
	}

	public static ConfigurationManager Instance { get; private set; }

	public WeaponConfiguration[] AvailableWeapons;
	public CarConfiguration[] AvailableVehicles;

	private void Awake()
	{
		ConfigurationManager.Instance = this;
		GameObject.DontDestroyOnLoad(this);
	}


	public bool PlayerOwned(string name, ConfigurationType type)
	{
		// Для тестов
		switch (type)
		{
			case ConfigurationType.Weapon:
				return this.AvailableWeapons.First(x => x.Name == name) != null;
			case ConfigurationType.Vehicle:
				return this.AvailableVehicles.First(x => x.Name == name) != null;
		}

		return false;
	}
}
