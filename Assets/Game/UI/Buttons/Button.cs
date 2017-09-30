using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Button : MonoBehaviour 
{
	[SerializeField] protected Color color;
	[SerializeField] protected Color pressedColor;
	[SerializeField] protected SpriteRenderer spriteRenderer;
	[SerializeField] protected TextMeshPro text;

	protected virtual void OnEnable(){}

	public void Introduce(float delay)
	{
		LowerButton();
		Invoke("RaiseButton", delay);
	}
	protected virtual void RaiseButton() 
	{
		spriteRenderer.color = color;
	}
	protected virtual void LowerButton()
	{
        spriteRenderer.color = pressedColor;
	}

	protected virtual void OnMouseDown()
	{
		if (spriteRenderer.color == color)
		{
            spriteRenderer.color = pressedColor;
		}
	}

	protected virtual void OnMouseUp()
	{
        if (spriteRenderer.color == pressedColor)
		{
            spriteRenderer.color = color;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{
				if (hit.collider.gameObject == this.gameObject)
				{
					OnPress();
				}
			}
		}
	}

	protected virtual void OnPress() {}
}
