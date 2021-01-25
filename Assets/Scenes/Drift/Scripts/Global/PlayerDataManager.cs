using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{


    /// <summary>
    /// Автомобили, которые доступны игроку
    /// </summary>
    public CarConfiguration[] PlayerOwnedCars;

	private void Awake()
	{


		this.LoadData();
	}

	public void SaveData()
	{

	}

    public void LoadData()
	{

	}
}
