using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SimpleBindableCollection : MonoBehaviour
{
	public MonoBehaviour CollectionContainer;
	public string PropertyName;
	public SimpleBindableElement UIElement;

	private void Start()
	{
		FieldInfo fi = this.CollectionContainer.GetType().GetField(this.PropertyName);
		if (fi == null)
		{
			Debug.LogError($"При биндинге коллекции {this.PropertyName} скрипта {this.GetType().Name} возникла ошибка. Не найдено свойство!");
			return;
		}

		if (fi.FieldType.BaseType != typeof(System.Array))
		{
			Debug.LogError($"Попытка забиндить свойство, которое не является коллекцией! Ошибочное свойство {this.PropertyName} скрипта " +
				$"{this.GetType().Name}.");
			return;
		}
		
		object[] objects =  (object[])fi.GetValue(this.CollectionContainer);

		foreach (object o in objects)
		{
			IBindable sbe = GameObject.Instantiate(this.UIElement, this.gameObject.transform);
			sbe.Bind(o);
		}
	}
}
