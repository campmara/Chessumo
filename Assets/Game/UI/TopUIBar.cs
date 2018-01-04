using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.iOS;

public class TopUIBar : MonoBehaviour 
{
	private RectTransform rect;

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
	}

	public void Introduce(float delay)
	{
		StartCoroutine(IntroduceRoutine(delay));
	}

	private IEnumerator IntroduceRoutine(float delay)
	{
		Vector3 startScale = new Vector3(1f, 0f, 1f);
		float desiredScaleY = 1f;

		if (Device.generation == DeviceGeneration.iPhoneX) // move top bar down if iphone x
		{
			rect.anchoredPosition = new Vector2(0f, -100f);
		}

		// Scale to 0 first.
		transform.localScale = startScale;

		// Then wait the delay.
		yield return new WaitForSeconds(delay);

		// Then rescale via tween.
		transform.DOScaleY(desiredScaleY, 1f).SetEase(Ease.OutBounce).OnComplete(OnComplete);

		yield return null;
	}

	private void OnComplete()
	{
		GameManager.Instance.restartButton.SetButtonEnabled(true);
		GameManager.Instance.restartButton.SetReadyForInput(true);
	}
}
