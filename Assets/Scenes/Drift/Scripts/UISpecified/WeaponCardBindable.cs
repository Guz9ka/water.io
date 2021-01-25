using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponCardBindable : SimpleBindableElement
{
	public Color OwnedColor;
	private Image image;

	private void Start()
	{

	}

	public override void Bind(object data)
	{
		this.image = this.GetComponent<Image>();
		WeaponConfiguration wc = (WeaponConfiguration)data;

		base.Bind(data);

		if (ConfigurationManager.Instance.PlayerOwned(wc.Name, ConfigurationManager.ConfigurationType.Weapon))
		{
			this.image.color = this.OwnedColor;
		}
	}
}
