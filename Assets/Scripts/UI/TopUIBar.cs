using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mara.MrTween;

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
        coverPanel.AlphaTo(0f, 1.5f)
            .SetEaseType(EaseType.Linear)
            .SetDelay(delay)
            .SetCompletionHandler((_) => {
                coverPanel.gameObject.SetActive(false);
                GameManager.Instance.restartButton.SetButtonEnabled(true);
                GameManager.Instance.restartButton.SetReadyForInput(true);
            })
            .Start();
    }
}
