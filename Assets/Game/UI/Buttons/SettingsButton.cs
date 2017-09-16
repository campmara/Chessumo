using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : Button 
{
	[SerializeField] private Sprite spriteO;
	[SerializeField] private Sprite spriteX;
	[SerializeField] private SpriteRenderer labelRenderer;

	private SettingsMenu settingsMenu;

	public void HookUpToMenu(SettingsMenu menu)
	{
		settingsMenu = menu;
		settingsMenu.button = this;
	}

	protected override void RaiseButton() 
	{
		Vector3 pos = labelRenderer.transform.localPosition;
		pos.y = 0.03f;
		labelRenderer.transform.localPosition = pos;

		spriteRenderer.sprite = upSprite;
	}
	protected override void LowerButton()
	{
		Vector3 pos = labelRenderer.transform.localPosition;
		pos.y = -0.03f;
		labelRenderer.transform.localPosition = pos;

		spriteRenderer.sprite = downSprite;
	}

	protected override void OnPress()
	{
		if (settingsMenu != null)
		{
			settingsMenu.Toggle();
		}
	}

	protected override void OnMouseDown()
	{
		if (spriteRenderer.sprite == upSprite)
		{
			spriteRenderer.sprite = downSprite;

			Vector3 pos = labelRenderer.transform.localPosition;
			pos.y -= 0.1f;
			labelRenderer.transform.localPosition = pos;
		}
	}

	protected override void OnMouseUp()
	{
		if (spriteRenderer.sprite == downSprite)
		{
			spriteRenderer.sprite = upSprite;

			Vector3 pos = labelRenderer.transform.localPosition;
			pos.y += 0.1f;
			labelRenderer.transform.localPosition = pos;

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

	public void SetO()
	{
		labelRenderer.sprite = spriteO;
	}

	public void SetX()
	{
		labelRenderer.sprite = spriteX;
	}
}
