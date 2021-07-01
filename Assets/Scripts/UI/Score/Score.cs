using UnityEngine;
using TMPro;

public class Score : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textMesh;

    private int score;
    public int GetScore() { return score; }

    void Awake() {
        score = 0;
        textMesh.text = "0";
    }

    public void SubmitScore() {
        SaveDataManager.Instance.TrySubmitHighScore(score);
        GameCenterManager.Instance.ReportScore(score);
    }

    public void ScorePoint() {
        score++;
        UpdateText();
    }

    public void Reset() {
        score = 0;
        UpdateText();
    }

    void UpdateText() {
        textMesh.text = score.ToString();
    }
}
