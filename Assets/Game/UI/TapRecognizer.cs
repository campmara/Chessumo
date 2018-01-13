using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapRecognizer : MonoBehaviour 
{
	public void OnTap()
	{
		if (UIManager.Instance.IsMenuOpen())
		{
			UIManager.Instance.ToggleMenu();
		}
		else
		{
			DontDoShit();
		}
	}

	private void DontDoShit()
	{
		
	}
}
