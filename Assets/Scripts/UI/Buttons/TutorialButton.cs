using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialButton : MonoBehaviour {
    public void OnPress() {
        AudioManager.Instance.PlayUIBlip();

        GameManager.Instance.restartButton.SetButtonEnabled(false);
        GameManager.Instance.restartButton.SetReadyForInput(false);

        // Loads the tutorial.
        TutorialManager.Instance.SetVisibility(true);
        GameManager.Instance.SetVisibility(false);
    }
}
