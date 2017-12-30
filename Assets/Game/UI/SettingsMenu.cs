using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour 
{
	public GameObject moreInfoPanel;

	bool inProgress = false;
	Tween tween;

	GameObject child;

	void Awake()
	{
		transform.localScale = new Vector3(0f, 0f, 0f);

		child = transform.GetChild(0).gameObject; // Gets the offset object.
		child.SetActive(false);
	}

    public void ImmediateToggle()
    {
        if (IsOpen())
        {
			GameManager.Instance.restartButton.SetReadyForInput(true);
            transform.localScale = Vector3.zero;
            child.SetActive(false);
        }
        else
        {
			GameManager.Instance.restartButton.SetReadyForInput(false);
            child.SetActive(true);
            transform.localScale = Vector3.one;
        }
    }

	public void Toggle()
	{
		ImmediateToggle();
		AudioManager.Instance.PlayUIBlip();
		/*
		if (inProgress) return;

		if (IsOpen())
		{
			StartCoroutine(Close());
		}
		else
		{
			StartCoroutine(Open());
		}
		*/
	}

	private IEnumerator Open()
	{
		GameManager.Instance.restartButton.SetReadyForInput(false);

		inProgress = true;
		child.SetActive(true);

		tween = transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

		yield return tween.WaitForCompletion();

		inProgress = false;
	}

	private IEnumerator Close()
	{
		GameManager.Instance.restartButton.SetReadyForInput(true);

		inProgress = true;

		tween = transform.DOScale(Vector3.zero, 1f).SetEase(Ease.OutQuint);

		yield return tween.WaitForCompletion();

		child.SetActive(false);
		inProgress = false;
	}

	public bool IsOpen()
	{
		return child.activeSelf;
	}

	public void OnCopyrightClicked()
	{
		moreInfoPanel.SetActive(true);
		AudioManager.Instance.PlayUIBlip();
	}

	public void OnPanelClicked()
	{
		moreInfoPanel.SetActive(false);
		AudioManager.Instance.PlayUIBlip();
	}
}
