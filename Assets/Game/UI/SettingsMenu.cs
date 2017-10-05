﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour 
{
	public SettingsButton button;

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
            transform.localScale = Vector3.zero;
            child.SetActive(false);
        }
        else
        {
            child.SetActive(true);
            transform.localScale = Vector3.one;
        }
    }

	public void Toggle()
	{
		if (inProgress) return;

		if (IsOpen())
		{
			StartCoroutine(Close());
		}
		else
		{
			StartCoroutine(Open());
		}
	}

	private IEnumerator Open()
	{
		inProgress = true;
		child.SetActive(true);

		tween = transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);

		yield return tween.WaitForCompletion();

		inProgress = false;
	}

	private IEnumerator Close()
	{
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
}
