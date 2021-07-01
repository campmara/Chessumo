using System.Collections;
using UnityEngine;
using Mara.MrTween;

public class SettingsMenu : MonoBehaviour {
    public GameObject moreInfoPanel;

    //`bool inProgress = false;
    ITween<Vector3> tween;

    GameObject child;

    void Awake() {
        transform.localScale = new Vector3(0f, 0f, 0f);

        child = transform.GetChild(0).gameObject; // Gets the offset object.
        child.SetActive(false);
    }

    public void ImmediateToggle() {
        if (IsOpen()) {
            GameManager.Instance.restartButton.SetReadyForInput(true);
            transform.localScale = Vector3.zero;
            child.SetActive(false);
        } else {
            GameManager.Instance.restartButton.SetReadyForInput(false);
            child.SetActive(true);
            transform.localScale = Vector3.one;
        }
    }

    public void Toggle() {
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

    private IEnumerator Open() {
        GameManager.Instance.restartButton.SetReadyForInput(false);

        //inProgress = true;
        child.SetActive(true);

        tween = transform.LocalScaleTo(Vector3.one, 1f).SetEaseType(EaseType.BackOut);
        tween.Start();
        yield return tween.WaitForCompletion();

        //inProgress = false;
    }

    private IEnumerator Close() {
        GameManager.Instance.restartButton.SetReadyForInput(true);

        //inProgress = true;

        tween = transform.LocalScaleTo(Vector3.zero, 1f).SetEaseType(EaseType.QuintOut);
        tween.Start();
        yield return tween.WaitForCompletion();

        child.SetActive(false);
        //inProgress = false;
    }

    public bool IsOpen() {
        return child.activeSelf;
    }

    public void OnCopyrightClicked() {
        moreInfoPanel.SetActive(true);
        AudioManager.Instance.PlayUIBlip();
    }

    public void OnPanelClicked() {
        moreInfoPanel.SetActive(false);
        AudioManager.Instance.PlayUIBlip();
    }
}
