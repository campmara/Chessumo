using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;

public class HighScore : MonoBehaviour 
{
	[SerializeField] private TextMeshPro textMesh;

	void Awake()
	{
		if (!textMesh)
		{
			Debug.LogError("Please assign the textMesh of the Score.");
		}
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMeFromSlit(this.gameObject, 1f, Ease.OutBounce);
	}

	public void PullHighScore()
	{
		textMesh.text = SaveDataManager.Instance.GetHighScore().ToString();
	}

	public void Reset()
	{
		textMesh.text = "0";
	}

	public Tween Lower()
	{
		return transform.DOMoveY(Constants.SCORE_LOWERED_Y, Constants.SCORE_TWEEN_TIME)
				.SetEase(Ease.Linear);
	}

	public Tween Raise()
	{
		return transform.DOMoveY(Constants.SCORE_RAISED_Y, Constants.SCORE_TWEEN_TIME)
				.SetEase(Ease.Linear);
	}
}

