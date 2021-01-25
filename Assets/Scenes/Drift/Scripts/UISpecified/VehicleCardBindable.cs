using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VehicleCardBindable : SimpleBindableElement
{
    public Color OwnedColor;
    public Button buyButton;
    public event EventHandler<EventArgs> OnVehicleCardActivated;
    
    private Image image;
    
    private void Start()
    {
        VehicleCardBindable[] bindableCards = FindObjectsOfType<VehicleCardBindable>();
        foreach (VehicleCardBindable card in bindableCards)
        {
            card.OnVehicleCardActivated += DeactivateButtonBuy;
            card.buyButton.gameObject.SetActive(false);
        }
    }

    public override void Bind(object data)
    {
        this.image = this.GetComponent<Image>();
        CarConfiguration cc = (CarConfiguration)data;

        base.Bind(data);

        if (ConfigurationManager.Instance.PlayerOwned(cc.Name, ConfigurationManager.ConfigurationType.Vehicle))
        {
            this.image.color = this.OwnedColor;
        }
    }

    public void ActivateButtonBuy()
    {
        OnVehicleCardActivated?.Invoke(this, EventArgs.Empty);
        buyButton.gameObject.SetActive(true);
    }

    public void DeactivateButtonBuy(object sender, EventArgs eventArgs)
    {
        buyButton.gameObject.SetActive(false);
    }
}
