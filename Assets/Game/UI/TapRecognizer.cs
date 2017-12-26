using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapRecognizer : MonoBehaviour 
{
	public void OnTap()
	{
		if (GameManager.Instance.settingsMenu.IsOpen())
		{
			GameManager.Instance.settingsMenu.Toggle();
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
