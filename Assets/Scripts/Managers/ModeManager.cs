using UnityEngine;
using System.Collections;

public class ModeManager : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// CONSTANTS
	/////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////
	// PUBLICS
	/////////////////////////////////////////////////////////////////////
	
	public static ModeManager Instance = null;

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////
	
	[SerializeField] Mode[] modePrefabs;

	public Mode CurrentMode { get { return currentMode; } }
	Mode currentMode;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		currentMode = null;
	}

	// Loads whichever mode is currently inhabiting currentMode.
	public void Load()
	{
		if (currentMode == null)
		{
			Debug.Log("[MODE MANAGER] Loading Mode: Classic");

			GameManager.Instance.Deselect();

			Mode mode = Instantiate(modePrefabs[0]) as Mode;
			mode.name = "Classic Mode";
			mode.transform.parent = transform;

			currentMode = mode;
			currentMode.Load();
		}
		else
		{
			Debug.Log("[MODE MANAGER] Unloading Mode: Classic");
			currentMode.Unload();

			currentMode = null;

			// Recursively go back and load the mode. This should only happen once.
			Load();
		}
	}
}
