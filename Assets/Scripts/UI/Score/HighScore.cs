using UnityEngine;
using TMPro;
using Mara.MrTween;

public class HighScore : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMesh;

    void OnEnable() {
        //GameManager.Instance.IntroduceFromSide(this.gameObject, 1.8f, true);
    }

    public void PullHighScore() {
        int score = SaveDataManager.Instance.GetHighScore();
        textMesh.text = score.ToString();
        GameCenterManager.Instance.ReportScore(score);
    }

    public void Reset() {
        textMesh.text = "0";
    }
}

