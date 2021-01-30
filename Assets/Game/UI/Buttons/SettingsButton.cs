using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsButton : MonoBehaviour {
    private SettingsMenu settingsMenu;

    public void HookUpToMenu(SettingsMenu menu) {
        settingsMenu = menu;
    }

    public void OnPress() {
        if (settingsMenu != null) {
            settingsMenu.Toggle();
        }
    }
}
