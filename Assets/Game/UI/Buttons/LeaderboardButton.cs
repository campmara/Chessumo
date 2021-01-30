using UnityEngine;
using UnityEngine.SocialPlatforms;

public class LeaderboardButton : MonoBehaviour {
    public void OnPress() {
        Social.ShowLeaderboardUI();
        AudioManager.Instance.PlayUIBlip();
    }
}
