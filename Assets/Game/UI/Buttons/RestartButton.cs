using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : MonoBehaviour 
{
	private bool gameStartedForTheFirstTime = false;
    private bool isTakingInput = false;

    public void SetReadyForInput(bool isReady)
    {
        isTakingInput = isReady;
    }

	void OnMouseDown()
	{
        if (!isTakingInput) return;

		// drag the circle trail effect around.
	}

	void OnMouseUp()
	{
        if (!isTakingInput) return;

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

	void OnPress() 
    {
		if (!gameStartedForTheFirstTime)
		{
			// Show the banner ad.
			AdManager.Instance.Banner.Show();
			gameStartedForTheFirstTime = true;
		}

		GameManager.Instance.BeginGame();
    }
}
