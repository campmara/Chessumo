using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PossibleMove : MonoBehaviour 
{
	void OnEnable()
	{
		StopAllCoroutines();
		StartCoroutine(GrowToFullSize());
	}

	void OnDisable()
	{
		StopAllCoroutines();
	}

	IEnumerator GrowToFullSize()
	{
		Vector3 desiredScale = Vector3.one;

		transform.localScale = new Vector3(0f, 0f, 1f);

		Tween tween = transform.DOScale(desiredScale, 1f)
			.SetEase(Ease.OutBack);

		yield return tween.WaitForCompletion();

		transform.localScale = desiredScale;
	}
}
