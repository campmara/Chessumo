using UnityEngine;
using System.Collections;
using TMPro;
using DG.Tweening;

public class HighScore : MonoBehaviour 
{
	[SerializeField] private TextMeshPro textMesh;

	private int score;

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
		GameManager.Instance.GrowMeFromSlit(this.gameObject, 1f);
	}

	void GetHighScore()
	{
		// Something something.
		// Pull the high score from my save manager.
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
}

