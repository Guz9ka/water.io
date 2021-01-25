using System;

public class PropertyChangedEventArgs : EventArgs
{
	public object OldValue { get; private set; }
	public object NewValue { get; private set; }

	/// <summary>
	/// Событие изменения свойства.
	/// </summary>
	/// <param name="oldValue">Старое значение</param>
	/// <param name="newValue">Новое значение</param>
	public PropertyChangedEventArgs(object oldValue, object newValue)
	{
		this.OldValue = oldValue;
		this.NewValue = newValue;
	}
}
