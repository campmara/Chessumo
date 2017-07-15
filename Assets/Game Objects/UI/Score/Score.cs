using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;
using UnityEngine.SocialPlatforms;

public class Score : MonoBehaviour 
{
	[SerializeField] private TextMeshPro textMesh;

	private int score;
	public int GetScore() { return score; }

	void Awake()
	{
		if (!textMesh)
		{
			Debug.LogError("Please assign the textMesh of the Score.");
		}

		score = 0;
		textMesh.text = "0";
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMeFromSlit(this.gameObject, 1f, Ease.OutBounce);
	}

	public void SubmitScore()
	{
		SaveDataManager.Instance.TrySubmitHighScore(score);
		GameCenterManager.Instance.ReportScore(score);
	}

	public void ScorePoint()
	{
		score++;
		UpdateText();
	}

	public void Reset()
	{
		score = 0;
		UpdateText();
	}

	void UpdateText()
	{
		textMesh.text = score.ToString();
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
