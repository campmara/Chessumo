using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundEffectsToggle : MonoBehaviour {
    private Toggle toggle;

    void Start() {
        toggle = GetComponent(typeof(Toggle)) as Toggle;

        toggle.isOn = AudioManager.Instance.SoundEnabled;
    }

    public void OnToggle() {
        SaveDataManager.Instance.ToggleSound();

        if (toggle.isOn) AudioManager.Instance.PlayUIBlip();
    }
}

