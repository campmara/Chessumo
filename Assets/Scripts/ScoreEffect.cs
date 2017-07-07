using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreEffect : MonoBehaviour 
{
	private SpriteRenderer sprite;
	private Tween fadeTween;

	private void Awake()
	{
		sprite = GetComponentInChildren(typeof(SpriteRenderer)) as SpriteRenderer;
	}

	public void OnPointScored()
	{
		if (fadeTween != null)
		{
			fadeTween.Kill();
			SetAlpha(0f);
		}

		float alpha = 1f;
		SetAlpha(alpha);

		fadeTween = DOTween.To(()=> alpha, x=> alpha = x, 0f, 0.75f)
			.OnUpdate(()=> SetAlpha(alpha));
	}

	private void SetAlpha(float alpha)
	{
		sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
	}
}
