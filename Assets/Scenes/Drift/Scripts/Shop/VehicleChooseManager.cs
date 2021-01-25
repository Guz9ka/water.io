using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleChooseManager : MonoBehaviour
{
    private ConfigurationManager configurationManager;

    private List<GameObject> shopContent;
    
    [Header("Параметры спавна транспорта")]
    [SerializeField] 
    private Transform vehicleSpawnPoint;
    [SerializeField] 
    private Quaternion vehicleSpawnRotation;

    private void Awake()
    {
        configurationManager = FindObjectOfType<ConfigurationManager>();
    }
    
    private void InitializeShopContent()
    {
        foreach (CarConfiguration vehicle in this.configurationManager.AvailableVehicles)
        {
            GameObject instance = GameObject.Instantiate(vehicle.CarPrefab, vehicleSpawnPoint.position, vehicleSpawnRotation);
            instance.GetComponent<PlayerController>().Deactivated = true;
            shopContent.Add(instance);
            
            //Заменить значение этой переменной на получение ID в списке машины игрока в данный момент
            string playerCurrentVehicleName = "Drochitel";
            if (vehicle.Name == playerCurrentVehicleName)
            {
                return;
            }
            
            instance.SetActive(false);
        }
    }

    public void ChooseVehicle(string vehicleName)
    {
        foreach (GameObject vehicle in shopContent)
        {
            if (vehicle.GetComponent<CarConfiguration>().Name != vehicleName)
            {
                vehicle.SetActive(false);
                return;
            }
            
            vehicle.SetActive(true);
        }
    }
}
