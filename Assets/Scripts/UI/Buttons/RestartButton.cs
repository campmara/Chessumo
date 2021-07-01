using System.Collections;
using UnityEngine;
using Mara.MrTween;

public class RestartButton : MonoBehaviour {
    [SerializeField] private SpriteRenderer touchDesignator;

    private const float MIN_S = 4f;
    private const float MAX_S = 5f;

    private const float FOLLOW_SPEED = 5f;

    private bool isTakingInput = false;

    private Coroutine pulseLoopRoutine = null;

    private TweenParty tweens = null;

    Vector3 followDir;

    private void Awake() {
        touchDesignator.gameObject.SetActive(false);
        touchDesignator.color = new Color(touchDesignator.color.r, touchDesignator.color.g, touchDesignator.color.b, 1f);
        touchDesignator.transform.localScale = new Vector3(MIN_S, MIN_S, MIN_S);

        //AdManager.Instance.OnAdShown += AdShown;
        //AdManager.Instance.OnAdClosed += AdClosed;
    }

    public void ResetEffect() {
        touchDesignator.color = new Color(touchDesignator.color.r, touchDesignator.color.g, touchDesignator.color.b, 1f);
        touchDesignator.transform.localScale = new Vector3(MIN_S, MIN_S, MIN_S);
    }

    public void AdShown() {
        SetButtonEnabled(false);
        SetReadyForInput(false);
    }

    public void AdClosed() {
        //SetButtonEnabled(true);
        //SetReadyForInput(true);
    }

    public void SetReadyForInput(bool isReady) {
        if (!touchDesignator.gameObject.activeSelf) return;

        Debug.Log("Ready for input? " + isReady);

        if (isReady) {
            touchDesignator.transform.position = new Vector3(Constants.I.GridOffsetX, Constants.I.GridOffsetY, 0f);
        }

        isTakingInput = isReady;
    }

    private IEnumerator PulseLoopRoutine() {
        AudioManager.Instance.PlaySuspense();
        ResetEffect();

        tweens = new TweenParty(1.5f);
        tweens.AddTween(
            touchDesignator.transform.LocalScaleTo(Vector3.one * MAX_S)
                .SetEaseType(EaseType.Linear)
        );
        tweens.AddTween(
            touchDesignator.AlphaTo(0f)
                .SetEaseType(EaseType.Linear)
        );

        tweens.Start();

        yield return tweens.WaitForCompletion();

        pulseLoopRoutine = StartCoroutine(PulseLoopRoutine());
    }

    public void KillPulse() {
        StopAllCoroutines();

        if (tweens != null && tweens.IsRunning()) {
            tweens.Stop();
            tweens = null;
        }
    }

    public void SetButtonEnabled(bool isEnabled) {
        Debug.Log("Button enabled? " + isEnabled);
        touchDesignator.gameObject.SetActive(isEnabled);
        GetComponent<BoxCollider>().enabled = isEnabled;

        if (isEnabled) {
            KillPulse();
            ResetEffect();
            pulseLoopRoutine = StartCoroutine(PulseLoopRoutine());
        } else {
            KillPulse();
        }
    }

    void OnMouseDown() {
        if (!isTakingInput) return;

        if (tweens != null && tweens.IsRunning()) {
            StopAllCoroutines();
            AudioManager.Instance.ReverseSuspense();
            tweens.ReverseTween();
        }
    }

    private void OnMouseUp() {
        if (!isTakingInput) return;

        OnPress();
    }

    private void OnPress() {
        AudioManager.Instance.PlayStartRelease();

        KillPulse();
        tweens = new TweenParty(1f);
        tweens.AddTween(
            touchDesignator.transform.LocalScaleTo(Vector3.one * MAX_S)
                .SetEaseType(EaseType.Linear)
        );
        tweens.AddTween(
            touchDesignator.AlphaTo(0f)
                .SetEaseType(EaseType.Linear)
        );
        tweens.SetCompletionHandler((_) => SetButtonEnabled(false));
        tweens.Start();

        SetReadyForInput(false);

        GameManager.Instance.BeginGame();
    }
}
