using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;

public class HighScore : MonoBehaviour 
{
	[SerializeField] private TextMeshProUGUI textMesh;

	void OnEnable()
	{
		//GameManager.Instance.IntroduceFromSide(this.gameObject, 1.8f, true);
	}

	public void PullHighScore()
	{
		int score = SaveDataManager.Instance.GetHighScore();
		textMesh.text = score.ToString();
		GameCenterManager.Instance.ReportScore(score);
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

