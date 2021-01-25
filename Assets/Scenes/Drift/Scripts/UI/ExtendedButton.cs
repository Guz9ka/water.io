using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Расширенная кнопка
/// </summary>
public class ExtendedButton : Button, IPointerEnterHandler, IPointerExitHandler
{
	/// <summary>
	/// Кнопка только с текстом.
	/// </summary>
	public bool IsTextButton = false;

	/// <summary>
	/// Цвет кнопки при наведении
	/// </summary>
	public Color HoverBackgroundColor;

	/// <summary>
	/// Стандартный цвет текста
	/// </summary>
	public Color DefaultForegroundColor;

	/// <summary>
	/// Цвет текста при наведении
	/// </summary>
	public Color HoverForegroundColor;

	/// <summary>
	/// Делать текст жирным при наведении
	/// </summary>
	public bool BoldOnHover = false;

	private TMP_Text text;
	private FontStyles defaultFontStyle;
	private Color defaultBackground;

	protected override void Start()
	{
		base.Start();

		if (this.IsTextButton)
		{
			this.text = this.GetComponentInChildren<TMP_Text>();
			this.defaultFontStyle = this.text.fontStyle;
		}
		else
		{
			if (this.targetGraphic != null)
			{
				this.defaultBackground = this.targetGraphic.color;
			}
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);

		if (this.IsTextButton)
		{
			this.text.color = this.HoverForegroundColor;

			if (this.BoldOnHover)
			{
				this.text.fontStyle = (this.defaultFontStyle | FontStyles.Bold);
			}
		}
		else
		{
			if (this.targetGraphic != null)
			{
				this.targetGraphic.color = this.HoverBackgroundColor;
			}
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);

		if (this.IsTextButton)
		{
			this.text.color = this.DefaultForegroundColor;
			this.text.fontStyle = this.defaultFontStyle;
		}
		else
		{
			if (this.targetGraphic != null)
			{
				this.targetGraphic.color = this.defaultBackground;
			}
		}
	}
}
