using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BatterySaverToggle : MonoBehaviour
{
	private Toggle toggle;

	void Awake()
	{
		toggle = GetComponent(typeof(Toggle)) as Toggle;

		toggle.isOn = SaveDataManager.Instance.IsBatterySaverOn();
	}

	public void OnToggle()
	{
		SaveDataManager.Instance.ToggleBatterySaver();
		AudioManager.Instance.PlayUIBlip();
	}
}
