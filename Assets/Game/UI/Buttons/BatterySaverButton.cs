using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterySaverButton : Button 
{
    [SerializeField] private Sprite unfilled;
    [SerializeField] private Sprite filled;

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
            spriteRenderer.sprite = filled;
		}
		else
		{
            spriteRenderer.sprite = unfilled;
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
