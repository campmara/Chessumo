using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScoreEffect : MonoBehaviour {
    [SerializeField] private SpriteRenderer bar;
    [SerializeField] private TextMeshPro textMesh;

    private const float START_Y = -3f;
    private const float ONE_Y_OFFSET = 0.5f;
    private const float TWO_Y_OFFSET = 0.25f;
    private const float THREE_Y_OFFSET = 0.25f;

    private float returnY = START_Y;

    private Tween tween;

    private void Awake() {
        transform.position = Vector3.up * START_Y;
    }

    public void SetPosition(Vector3 pos) {
        transform.position = pos;
        returnY = pos.y;
    }

    public void OnOneScored(Color col) {
        if (tween != null) {
            tween.Kill();
        }

        AudioManager.Instance.PlayScoreOneNote();

        bar.color = col;
        textMesh.text = "+" + Constants.I.ScoreOneAmount.ToString();

        tween = transform.DOMoveY(transform.position.y + ONE_Y_OFFSET, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(OnTweenComplete);
    }

    public void OnTwoScored(Color col) {
        if (tween != null) {
            tween.Kill();
        }

        AudioManager.Instance.PlayScoreTwoNote();

        bar.DOColor(col, 0.1f).SetEase(Ease.Linear);
        textMesh.text = "+" + Constants.I.ScoreTwoAmount.ToString();

        tween = transform.DOMoveY(transform.position.y + TWO_Y_OFFSET, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(OnTweenComplete);
    }

    public void OnThreeScored(Color col) {
        if (tween != null) {
            tween.Kill();
        }

        AudioManager.Instance.PlayScoreThreeNote();

        bar.DOColor(col, 0.1f).SetEase(Ease.Linear);
        textMesh.text = "+" + Constants.I.ScoreThreeAmount.ToString();

        tween = transform.DOMoveY(transform.position.y + THREE_Y_OFFSET, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(OnTweenComplete);
    }

    public void OnTweenComplete() {
        tween = transform.DOMoveY(returnY, 0.5f).SetEase(Ease.InQuint);
    }
}
