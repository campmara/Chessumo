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
		GameManager.Instance.GrowMe(this.gameObject, 1.5f, Ease.OutBounce);
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
		return transform.DOMoveY(Constants.I.ScoreLoweredY, Constants.I.ScoreTweenTime)
				.SetEase(Ease.Linear);
	}

	public Tween Raise()
	{
		return transform.DOMoveY(Constants.I.ScoreRaisedY, Constants.I.ScoreTweenTime)
				.SetEase(Ease.Linear);
	}
}

