using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// CONSTANTS
	/////////////////////////////////////////////////////////////////////

	private const string HIGH_SCORE_KEY = "HIGHSCORE";
	private const string BATTERY_SAVER_KEY = "BATTERY_SAVER";
    private const string TUTORIAL_COMPLETE_KEY = "TUTORIAL_COMPLETE";
	private const string ADS_REMOVED_KEY = "ADS_REMOVED";

	/////////////////////////////////////////////////////////////////////
	// PUBLICS
	/////////////////////////////////////////////////////////////////////

	public static SaveDataManager Instance = null;

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////
	// FUNCTIONS
	/////////////////////////////////////////////////////////////////////

	void Awake()
	{
		if (!Instance)
		{
			Instance = this;
		}
	}

	// HIGH SCORE

	public void TrySubmitHighScore(int score)
	{
		if (IsHighScore(score))
		{
			SetHighScore(score);
			Debug.Log("New high score submitted.");
		}
		else
		{
			Debug.Log("Score is not high score.");
		}
	}

	bool IsHighScore(int score)
	{
		return score > PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
	}

	void SetHighScore(int score)
	{
		PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
		PlayerPrefs.Save();
	}

	public int GetHighScore()
	{
		return PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
	}

	// BATTERY SAVER

	public void ToggleBatterySaver()
	{
		int num = PlayerPrefs.GetInt(BATTERY_SAVER_KEY, -1);

		if (num == 0) // Set to false
		{
			PlayerPrefs.SetInt(BATTERY_SAVER_KEY, 1);
            PlayerPrefs.Save();
			Application.targetFrameRate = 30;
		}
		else if (num == 1) // Set to true
		{
			PlayerPrefs.SetInt(BATTERY_SAVER_KEY, 0);
            PlayerPrefs.Save();
			Application.targetFrameRate = 60;
		}
		else if (num == -1) // First time setup.
		{
			PlayerPrefs.SetInt(BATTERY_SAVER_KEY, 0);
            PlayerPrefs.Save();
			Application.targetFrameRate = 60;
		}
	}

	public bool IsBatterySaverOn()
	{
		return PlayerPrefs.GetInt(BATTERY_SAVER_KEY, -1) == 1;
	}

    // TUTORIAL COMPLETION

    public void OnTutorialComplete()
    {
        PlayerPrefs.SetInt(TUTORIAL_COMPLETE_KEY, 1);
        PlayerPrefs.Save();
    }

    // used to check if we should load the tutorial on start.
    public bool IsTutorialComplete()
    {
        return PlayerPrefs.GetInt(TUTORIAL_COMPLETE_KEY, 0) == 1 ? true : false;
    }

	// REMOVE ADS

	public void OnPayToRemoveAds()
	{
		PlayerPrefs.SetInt(ADS_REMOVED_KEY, 1);
	}

	public bool HasPaidToRemoveAds()
	{
		return PlayerPrefs.GetInt(ADS_REMOVED_KEY, 0) == 1 ? true : false;
	}
}
