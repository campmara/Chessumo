using UnityEngine;
using TMPro;
using Mara.MrTween;

public class ScoreEffect : MonoBehaviour {
    [SerializeField] private SpriteRenderer bar;
    [SerializeField] private TextMeshPro textMesh;

    private const float START_Y = -3f;
    private const float ONE_Y_OFFSET = 0.5f;
    private const float TWO_Y_OFFSET = 0.25f;
    private const float THREE_Y_OFFSET = 0.25f;

    private float returnY = START_Y;

    private TweenChain scoreTweens = null;

    private void Awake() {
        transform.position = Vector3.up * START_Y;
    }

    public void SetPosition(Vector3 pos) {
        transform.position = pos;
        returnY = pos.y;
    }

    public void OnOneScored(Color col) {
        AudioManager.Instance.PlayScoreOneNote();

        bar.color = col;
        textMesh.text = "+" + Constants.I.ScoreOneAmount.ToString();

        TweenToOffset(ONE_Y_OFFSET);
    }

    public void OnTwoScored(Color col) {
        AudioManager.Instance.PlayScoreTwoNote();

        bar.ColorTo(col, 0.1f).SetEaseType(EaseType.Linear).Start();
        textMesh.text = "+" + Constants.I.ScoreTwoAmount.ToString();

        TweenToOffset(TWO_Y_OFFSET);
    }

    public void OnThreeScored(Color col) {
        AudioManager.Instance.PlayScoreThreeNote();

        bar.ColorTo(col, 0.1f).SetEaseType(EaseType.Linear).Start();
        textMesh.text = "+" + Constants.I.ScoreThreeAmount.ToString();

        TweenToOffset(THREE_Y_OFFSET);
    }

    private void TweenToOffset(float yOffset) {
        if (scoreTweens != null && scoreTweens.IsRunning()) {
            scoreTweens.Stop();
            scoreTweens = null;
        }

        scoreTweens = new TweenChain();

        scoreTweens.AppendTween(
            transform.YPositionTo(transform.position.y + yOffset, 0.5f)
                .SetEaseType(EaseType.QuintOut)
        );

        scoreTweens.AppendTween(
            transform.YPositionTo(returnY, 0.5f)
                .SetEaseType(EaseType.QuintIn)
        );

        scoreTweens.Start();
    }
}
