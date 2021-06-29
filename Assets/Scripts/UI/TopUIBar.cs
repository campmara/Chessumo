using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class TopUIBar : MonoBehaviour {
    [SerializeField] private RawImage coverPanel;
    private RectTransform barRect;

    private void Awake() {
        barRect = (RectTransform)this.transform;

        // ensure that top ui bar is in device safe area
        Rect safe = Screen.safeArea;
        barRect.anchoredPosition = new Vector2(0f, -safe.yMin);
    }

    public void Introduce(float delay) {
        StartCoroutine(IntroduceRoutine(delay));
    }

    private IEnumerator IntroduceRoutine(float delay) {
        yield return new WaitForSeconds(delay);
        coverPanel.DOFade(0f, 1.5f).SetEase(Ease.Linear).OnComplete(OnComplete);
    }

    private void OnComplete() {
        coverPanel.gameObject.SetActive(false);
        GameManager.Instance.restartButton.SetButtonEnabled(true);
        GameManager.Instance.restartButton.SetReadyForInput(true);
    }
}
