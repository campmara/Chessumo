using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class GameEndMessage : MonoBehaviour 
{
	[SerializeField] private TextMeshPro textMesh;

	private string[] messages = new string[]
	{
		"Nice.",
		"Well, heck.",
		"That's all!",
		"Nice job.",
		"Cool."
	};

	private void Awake()
	{
		transform.position = new Vector3(0f, Constants.ENDMSG_RAISED_Y, 0f);
	}

	public Tween Appear()
	{
		// Set a random message from the list.
		textMesh.text = messages[Random.Range(0, messages.Length)];

		// Tween it down!
		return transform.DOMoveY(Constants.ENDMSG_LOWERED_Y, Constants.ENDMSG_TWEEN_TIME)
				.SetEase(Ease.OutBounce);
	}

	public Tween Disappear()
	{
		return transform.DOMoveY(Constants.ENDMSG_RAISED_Y, Constants.ENDMSG_TWEEN_TIME)
				.SetEase(Ease.InQuint);
	}
}
