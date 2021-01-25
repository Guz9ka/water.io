using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleBindableElement : MonoBehaviour, IBindable
{
	[Serializable]
	public struct BindableTextProperty
	{
		public string Name;
		public string Format;
		public TMP_Text TextElement;

		public BindableTextProperty(string name, string format, TMP_Text textElement)
		{
			this.Name = name;
			this.Format = format;
			this.TextElement = textElement;

			if (string.IsNullOrEmpty(format))
			{
				this.Format = "{0}";
			}
		}
	}

	[Serializable]
	public struct BindableImageProperty
	{
		public string Name;
		public Image ImageElement;

		public BindableImageProperty(string name, string format, Image imageElement)
		{
			this.Name = name;
			this.ImageElement = imageElement;
		}
	}


	public BindableTextProperty[] TextProperties;
	public BindableImageProperty[] ImageProperties;

	public virtual void Bind(object data)
	{
		foreach (BindableTextProperty bindableProp in this.TextProperties)
		{
			FieldInfo fi = data.GetType().GetField(bindableProp.Name);
			if (fi == null)
			{
				Debug.LogError($"При биндинге свойства {bindableProp.Name} скрипта {this.GetType().Name} возникла ошибка. Не найдено свойство!");
				continue;
			}

			bindableProp.TextElement.text = string.Format(bindableProp.Format, fi.GetValue(data));
		}

		foreach (BindableImageProperty bindableProp in this.ImageProperties)
		{
			FieldInfo fi = data.GetType().GetField(bindableProp.Name);
			if (fi == null)
			{
				Debug.LogError($"При биндинге свойства {bindableProp.Name} скрипта {this.GetType().Name} возникла ошибка. Не найдено свойство!");
				continue;
			}

			bindableProp.ImageElement.sprite = (Sprite)fi.GetValue(data);
		}
	}
}
