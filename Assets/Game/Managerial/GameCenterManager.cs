using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameCenterManager : Singleton<GameCenterManager> {
    private const string GAMECENTER_LEADERBOARD_ID = "grp.chessumoHighScores";

    void Start() {
        // Authenticate and register a ProcessAuthentication callback
        // This call needs to be made before we can proceed to other calls in the Social API
        Social.localUser.Authenticate(ProcessAuthentication);
    }

    // This function gets called when Authenticate completes
    // Note that if the operation is successful, Social.localUser will contain data from the server. 
    void ProcessAuthentication(bool success) {
        if (success) {
            Debug.Log("Authenticated.");
        } else {
            Debug.Log("Failed to authenticate.");
        }
    }

    public void ReportScore(int score) {
        long scoreLong = score;

#if !UNITY_EDITOR
        try {
            Social.ReportScore(scoreLong, GAMECENTER_LEADERBOARD_ID, HighScoreCheck);
        } catch {
            Debug.Log("Report Score to Game Center Failed");
        }
#endif
    }

    void HighScoreCheck(bool result) {
        if (result) {
            Debug.Log("score submission successful");
        } else {
            Debug.Log("score submission failed");
        }
    }

    public int GetHighScore() {
        int score = 0;
        ILeaderboard leaderboard = Social.CreateLeaderboard();
        leaderboard.id = GAMECENTER_LEADERBOARD_ID;
        leaderboard.timeScope = TimeScope.Today;

        leaderboard.LoadScores(success => {
            IScore iScore = leaderboard.localUserScore;
            if (iScore != null) {
                score = (int)iScore.value;
            }
        });

        return score;
    }
}
