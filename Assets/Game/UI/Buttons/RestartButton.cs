using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RestartButton : MonoBehaviour 
{
    [SerializeField] private SpriteRenderer touchDesignator;

    private const float MIN_S = 4f;
    private const float MAX_S = 5f;

    private const float FOLLOW_SPEED = 5f;

    private bool isTakingInput = false;

    private Coroutine pulseLoopRoutine = null;

    private Tween fadeTween = null;
    private Tween pulseTween = null;

    Vector3 followDir;

    private void Awake()
    {
        touchDesignator.gameObject.SetActive(false);
        touchDesignator.color = new Color(touchDesignator.color.r, touchDesignator.color.g, touchDesignator.color.b, 1f);
        touchDesignator.transform.localScale = new Vector3(MIN_S, MIN_S, MIN_S);

        AdManager.Instance.OnAdShown += AdShown;
        AdManager.Instance.OnAdClosed += AdClosed;
    }

    public void ResetEffect()
    {
        touchDesignator.color = new Color(touchDesignator.color.r, touchDesignator.color.g, touchDesignator.color.b, 1f);
        touchDesignator.transform.localScale = new Vector3(MIN_S, MIN_S, MIN_S);
    }

    public void AdShown()
    {
        SetButtonEnabled(false);
        SetReadyForInput(false);
    }

    public void AdClosed()
    {
        SetButtonEnabled(true);
        SetReadyForInput(true);
    }

    public void SetReadyForInput(bool isReady)
    {
        if (!touchDesignator.gameObject.activeSelf) return;

        Debug.Log("Ready for input? " + isReady);

        if (isReady)
        {
            KillPulse();

            touchDesignator.transform.position = new Vector3(Constants.I.GridOffsetX, Constants.I.GridOffsetY, 0f);
            ResetEffect();

            pulseLoopRoutine = StartCoroutine(PulseLoopRoutine());
        }

        isTakingInput = isReady;
    }

    private IEnumerator PulseLoopRoutine()
    {
        AudioManager.Instance.PlaySuspense();
        ResetEffect();

        pulseTween = touchDesignator.transform.DOScale(Vector3.one * MAX_S, 2f);
        fadeTween = touchDesignator.DOFade(0f, 2f);

        yield return fadeTween.WaitForCompletion();

        pulseLoopRoutine = StartCoroutine(PulseLoopRoutine());
    }

    public void KillPulse()
    {
        StopAllCoroutines();
        pulseTween.Kill();
        fadeTween.Kill();
    }

    public void SetButtonEnabled(bool isEnabled)
    {
        touchDesignator.gameObject.SetActive(isEnabled);
        GetComponent<BoxCollider>().enabled = isEnabled;
    }

	void OnMouseDown()
	{
        if (!isTakingInput) return;

        KillPulse();
        float time = AudioManager.Instance.ReverseSuspense();

        pulseTween = touchDesignator.transform.DOScale(Vector3.one * MIN_S, time);
        fadeTween = touchDesignator.DOFade(1f, time);
	}

	void OnMouseUp()
	{
        if (!isTakingInput) return;

        OnPress();
	}

    void OnEffectFinish()
    {
        SetButtonEnabled(false);
    }

	void OnPress() 
    {
        AudioManager.Instance.PlayStartRelease();

        touchDesignator.transform.DOScale(Vector3.one * MAX_S, 1f);
        touchDesignator.DOFade(0f, 1f).OnComplete(OnEffectFinish);

        SetReadyForInput(false);

		GameManager.Instance.BeginGame();
    }
}
