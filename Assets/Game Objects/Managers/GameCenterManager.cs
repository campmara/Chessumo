using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameCenterManager : MonoBehaviour 
{
    public static GameCenterManager Instance = null;

    private const string GAMECENTER_LEADERBOARD_ID = "yourLeaderboardIDinQuotes";

	private void Awake()
	{
        if (Instance == null)
		{
			Instance = this;
		}
	}

	void Start () {
        // Authenticate and register a ProcessAuthentication callback
        // This call needs to be made before we can proceed to other calls in the Social API
        Social.localUser.Authenticate (ProcessAuthentication);
    }

    // This function gets called when Authenticate completes
    // Note that if the operation is successful, Social.localUser will contain data from the server. 
    void ProcessAuthentication (bool success) 
    {
        if (success) 
        {
            Debug.Log ("Authenticated.");
        }
        else
        {
            Debug.Log ("Failed to authenticate.");
        }
    }

    public void ReportScore(int score)
    {
        long scoreLong = score;
        
        //Social.ReportScore(scoreLong, GAMECENTER_LEADERBOARD_ID, HighScoreCheck);
    }
    
    void HighScoreCheck(bool result) 
    {
        if(result)
        {
            Debug.Log("score submission successful");
        }
        else
        {
            Debug.Log("score submission failed");
        }
    }
}
