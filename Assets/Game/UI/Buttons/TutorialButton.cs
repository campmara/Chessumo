using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : Button 
{
	protected override void OnPress()
	{
        // Loads the tutorial.
        TutorialManager.Instance.SetVisibility(true);
        GameManager.Instance.SetVisibility(false);
	}
}
