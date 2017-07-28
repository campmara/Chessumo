using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Button : MonoBehaviour 
{
	[SerializeField] protected Sprite upSprite;
	[SerializeField] protected Sprite downSprite;
	protected SpriteRenderer spriteRenderer;
	protected TextMeshPro text;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren(typeof(SpriteRenderer)) as SpriteRenderer;
		text = GetComponentInChildren(typeof(TextMeshPro)) as TextMeshPro;
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMe(this.gameObject, 1.5f, Ease.OutBounce);
	}

	private void OnMouseDown()
	{
		if (spriteRenderer.sprite == upSprite)
		{
			spriteRenderer.sprite = downSprite;

			Vector3 pos = text.transform.localPosition;
			pos.y -= 0.1f;
			text.transform.localPosition = pos;
		}
	}

	private void OnMouseUp()
	{
		if (spriteRenderer.sprite == downSprite)
		{
			spriteRenderer.sprite = upSprite;

			Vector3 pos = text.transform.localPosition;
			pos.y += 0.1f;
			text.transform.localPosition = pos;

			OnPress();
		}
	}

	protected virtual void OnPress() {}
}
