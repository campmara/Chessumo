using UnityEngine;
using DG.Tweening;

public class DebugStartButton : MonoBehaviour 
{
	[SerializeField] Sprite upSprite;
	[SerializeField] Sprite downSprite;

	SpriteRenderer spriteRenderer;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		spriteRenderer.sprite = downSprite;
	}

	void OnMouseDown()
	{
		if (spriteRenderer.sprite == upSprite)
		{
			spriteRenderer.sprite = downSprite;
			GameManager.Instance.BeginGame();
			Invoke("Lower", 3f);
		}
	}

	public void Raise()
	{
		transform.DOMoveY(Constants.QUIT_RAISED_Y, Constants.QUIT_TWEEN_TIME)
			.SetEase(Ease.OutBack);

		Invoke("RaiseButton", Constants.QUIT_TWEEN_TIME + 0.25f);
	}

	public void Lower()
	{
		transform.DOMoveY(Constants.QUIT_LOWERED_Y, Constants.QUIT_TWEEN_TIME)
			.SetEase(Ease.InQuint);
	}

	void RaiseButton()
	{
		spriteRenderer.sprite = upSprite;
	}
}
