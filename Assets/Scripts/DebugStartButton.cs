using UnityEngine;
using System.Collections;

public class DebugStartButton : MonoBehaviour 
{
	void OnMouseDown()
	{
		GameManager.Instance.BeginGame();
	}
}
