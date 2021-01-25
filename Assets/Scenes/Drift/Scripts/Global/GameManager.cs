using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region singleton
	private static GameManager _instance;
	public static GameManager Instance
	{
		get
		{
			return GameManager._instance;
		}
	}
	#endregion

	public event EventHandler<PropertyChangedEventArgs> OnPropertyChanged;

	private long _score;
	/// <summary>
	/// Кол-во очков пользователя за сессию
	/// </summary>
	public long Score 
	{
		get
		{
			return this._score;
		}
		set
		{
			long oldValue = this._score;
			this._score = value;

			this.PropertyChanged(nameof(this.Score), new PropertyChangedEventArgs(oldValue, value));
		}
	}

	private int _playerLives = 1;
	/// <summary>
	/// Кол-во доступных респавнов игрока
	/// </summary>
	public int PlayerLives
	{
		get
		{
			return this._playerLives;
		}
		set
		{
			long oldValue = this._playerLives;
			this._playerLives = value;

			this.PropertyChanged(nameof(this.PlayerLives), new PropertyChangedEventArgs(oldValue, value));
		}
	}

	private void Awake()
	{
		GameManager._instance = this;
	}


	protected virtual void PropertyChanged(object sender, PropertyChangedEventArgs args)
	{
		Debug.Log($"[GAMEMANAGER]: Изменилось свойство '{(string)sender}'. Старое значение: {args.OldValue}; Новое значение: {args.NewValue}");
		this.OnPropertyChanged?.Invoke(sender, args);
	}
}
