using System.Collections;
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
		transform.localScale = new Vector3(1f, 0f, 1f);

		child = transform.GetChild(0).gameObject; // Gets the offset object.
		child.SetActive(false);
	}

    public void ImmediateToggle()
    {
        if (IsOpen())
        {
            transform.localScale = new Vector3(transform.localScale.x, 0f, transform.localScale.z);
            button.SetO();
            child.SetActive(false);
        }
        else
        {
            child.SetActive(true);
            transform.localScale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);
            button.SetX();
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

		tween = transform.DOScaleY(1f, 1f).SetEase(Ease.OutBounce);

		yield return tween.WaitForCompletion();

		button.SetX();
		inProgress = false;
	}

	private IEnumerator Close()
	{
		inProgress = true;

		tween = transform.DOScaleY(0f, 1f).SetEase(Ease.OutQuint);

		yield return tween.WaitForCompletion();

		button.SetO();
		child.SetActive(false);
		inProgress = false;
	}

	public bool IsOpen()
	{
		return child.activeSelf;
	}
}
