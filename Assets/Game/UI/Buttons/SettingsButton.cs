using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : Button 
{
	private SettingsMenu settingsMenu;

	public void HookUpToMenu(SettingsMenu menu)
	{
		settingsMenu = menu;
		settingsMenu.button = this;
	}

	protected override void OnPress()
	{
		if (settingsMenu != null)
		{
			settingsMenu.Toggle();
		}
	}
}
