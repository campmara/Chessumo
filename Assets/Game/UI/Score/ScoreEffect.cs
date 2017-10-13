using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScoreEffect : MonoBehaviour 
{
    [SerializeField] private SpriteRenderer bar;
    [SerializeField] private TextMeshPro textMesh;

    private const float START_Y = -3f;
    private const float ONE_Y = -2.5f;
    private const float TWO_Y = -2.25f;
    private const float THREE_Y = -2f;

    private Tween tween;

	private void Awake()
	{
        transform.position = Vector3.up * START_Y;
	}

    public void OnOneScored(Color col)
    {
        if (tween != null)
        {
            tween.Kill();
        }

        bar.color = col;
        textMesh.text = "+" + Constants.I.ScoreOneAmount.ToString();

        tween = transform.DOMoveY(ONE_Y, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(OnTweenComplete);
    }

	public void OnTwoScored(Color col)
	{
        if (tween != null)
        {
            tween.Kill();
        }

        bar.DOColor(col, 0.1f).SetEase(Ease.Linear);
        textMesh.text = "+" + Constants.I.ScoreTwoAmount.ToString();

        tween = transform.DOMoveY(TWO_Y, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(OnTweenComplete);
	}

	public void OnThreeScored(Color col)
	{
        if (tween != null)
        {
            tween.Kill();
        }

        bar.DOColor(col, 0.1f).SetEase(Ease.Linear);
        textMesh.text = "+" + Constants.I.ScoreThreeAmount.ToString();

        tween = transform.DOMoveY(THREE_Y, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(OnTweenComplete);
	}

    public void OnTweenComplete()
	{
        tween = transform.DOMoveY(START_Y, 0.5f).SetEase(Ease.InQuint);
	}
}
