using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreEffect : MonoBehaviour 
{
    [SerializeField] private SpriteRenderer one;
    [SerializeField] private SpriteRenderer two;
    [SerializeField] private SpriteRenderer three;

    private const float DESIRED_Y = 0f;

    private Tween oneTween;
    private Tween twoTween;
    private Tween threeTween;

	private void Awake()
	{
        one.transform.localPosition = Vector3.down * 2f;
        two.transform.localPosition = Vector3.down * 2f;
        three.transform.localPosition = Vector3.down * 2f;
	}

    public void OnOneScored(Color col)
    {
        if (oneTween != null)
        {
            oneTween.Kill();
            one.transform.localPosition = Vector3.down * 2f;
        }

        one.color = col;

        oneTween = one.transform.DOLocalMoveY(DESIRED_Y, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(() => oneTween = one.transform.DOLocalMoveY(-2f, 0.5f).SetEase(Ease.InQuint));
    }

	public void OnTwoScored(Color col)
	{
        if (twoTween != null)
		{
			twoTween.Kill();
			two.transform.localPosition = Vector3.down * 2f;
		}

		two.color = col;

        twoTween = two.transform.DOLocalMoveY(DESIRED_Y, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(() => twoTween = two.transform.DOLocalMoveY(-2f, 0.5f).SetEase(Ease.InQuint));
	}

	public void OnThreeScored(Color col)
	{
		if (threeTween != null)
		{
			threeTween.Kill();
			three.transform.localPosition = Vector3.down * 2f;
		}

		three.color = col;

        threeTween = three.transform.DOLocalMoveY(DESIRED_Y, 0.5f)
                      .SetEase(Ease.OutQuint)
                      .OnComplete(() => threeTween = three.transform.DOLocalMoveY(-2f, 0.5f).SetEase(Ease.InQuint));
	}
}
