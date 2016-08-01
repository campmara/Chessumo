using UnityEngine;
using System.Collections;
using TMPro;

public class Score : MonoBehaviour 
{
	TextMeshPro textMesh;

	int score;

	void Awake()
	{
		textMesh = GetComponentInChildren<TextMeshPro>();
		score = 0;
		textMesh.text = "0";
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
}
