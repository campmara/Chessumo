using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySaverButton : Button 
{
	void Start()
	{
		UpdateCheckbox();
	}

	protected override void OnMouseDown()
	{
		SaveDataManager.Instance.ToggleBatterySaver();
		UpdateCheckbox();
	}

	private void UpdateCheckbox()
	{
		if (SaveDataManager.Instance.IsBatterySaverOn())
		{
			spriteRenderer.sprite = downSprite;
		}
		else
		{
			spriteRenderer.sprite = upSprite;
		}
	}

	protected override void OnMouseUp()
	{
		// nothing.
	}

	protected override void OnPress()
	{
		
	}
}
