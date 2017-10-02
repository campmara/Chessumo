using UnityEngine;
using System.Collections;
using DG.Tweening;

public class TopUIBar : MonoBehaviour 
{
	public void Introduce(float delay)
	{
		StartCoroutine(IntroduceRoutine(delay));
	}

	private IEnumerator IntroduceRoutine(float delay)
	{
		Vector3 startScale = new Vector3(1f, 0f, 1f);
		float desiredScaleY = 1f;

		// Scale to 0 first.
		transform.localScale = startScale;

		// Then wait the delay.
		yield return new WaitForSeconds(delay);

		// Then rescale via tween.
		transform.DOScaleY(desiredScaleY, 1f).SetEase(Ease.OutBounce).OnComplete(() => GameManager.Instance.restartButton.SetReadyForInput(true));

		yield return null;
	}
}
