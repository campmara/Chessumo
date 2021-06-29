using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public static UIManager Instance = null;

    [Header("References")]
    [SerializeField] private TopUIBar topUIBar;
    [SerializeField] private Score score;
    [SerializeField] private HighScore highScore;
    [SerializeField] private LeaderboardButton leaderboardButton;
    [SerializeField] private SettingsButton settingsButton;
    [SerializeField] private SettingsMenu settingsMenu;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }

    public void SetVisibility(bool isVisible) {
        topUIBar.gameObject.SetActive(isVisible);

        if (!isVisible) {
            if (settingsMenu.IsOpen()) settingsMenu.ImmediateToggle();
        }

        settingsMenu.gameObject.SetActive(isVisible);
    }

    /*
		GAME STUFF
	*/

    public void IntroduceTopBar(float delay) {
        topUIBar.Introduce(1f);
    }

    public bool IsMenuOpen() {
        return settingsMenu.IsOpen();
    }

    public void ToggleMenu() {
        settingsMenu.Toggle();
    }

    public void Initialize() {
        settingsButton.HookUpToMenu(settingsMenu);

        score.Reset();
        highScore.PullHighScore();
    }

    public void SubmitFinalScore() {
        score.SubmitScore();
        highScore.PullHighScore();
    }

    public void OnGameRestarted() {
        score.SubmitScore();
        highScore.PullHighScore();
        score.Reset();
    }

    public void ScorePoints(int amount) {
        for (int i = 0; i < amount; i++) {
            score.ScorePoint();
        }
    }

    /*
		TUTORIAL STUFF
	*/


}
