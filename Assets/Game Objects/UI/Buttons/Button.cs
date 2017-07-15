using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour 
{
	[SerializeField] protected Sprite upSprite;
	[SerializeField] protected Sprite downSprite;
	protected SpriteRenderer spriteRenderer;

	private void OnMouseDown()
	{
		if (spriteRenderer.sprite == upSprite)
		{
			spriteRenderer.sprite = downSprite;
		}
	}

	private void OnMouseUp()
	{
		if (spriteRenderer.sprite == downSprite)
		{
			spriteRenderer.sprite = upSprite;
		}
	}
}
