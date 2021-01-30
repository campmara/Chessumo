using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsRestartButton : MonoBehaviour {
    private int numTimesReset = 0;

    public void OnPress() {
        AudioManager.Instance.PlayUIBlip();
        GameManager.Instance.StartNewGame();
        UIManager.Instance.ToggleMenu();

        numTimesReset++;
        if (numTimesReset >= 4) {
            GameManager.Instance.restartButton.KillPulse();
            //AdManager.Instance.TryShowVideoAd();
            numTimesReset = 0;
        }
    }
}
