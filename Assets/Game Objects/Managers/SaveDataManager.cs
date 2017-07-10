using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataManager : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// CONSTANTS
	/////////////////////////////////////////////////////////////////////

	private const string HIGH_SCORE_KEY = "HIGHSCORE";

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
}
