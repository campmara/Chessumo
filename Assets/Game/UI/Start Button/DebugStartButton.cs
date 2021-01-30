using UnityEngine;
using DG.Tweening;

public class DebugStartButton : MonoBehaviour {
    [SerializeField] Sprite upSprite;
    [SerializeField] Sprite downSprite;

    SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = downSprite;
    }

    void OnMouseDown() {
        if (spriteRenderer.sprite == upSprite) {
            spriteRenderer.sprite = downSprite;
            GameManager.Instance.BeginGame();
            Invoke("Lower", 3f);
        }
    }

    public void Raise() {
        transform.DOMoveY(Constants.I.StartButtonRaisedY, Constants.I.StartButtonTweenTime)
            .SetEase(Ease.OutBack);

        Invoke("RaiseButton", Constants.I.StartButtonTweenTime + 0.25f);
    }

    public void Lower() {
        transform.DOMoveY(Constants.I.StartButtonLoweredY, Constants.I.StartButtonTweenTime)
            .SetEase(Ease.InQuint);
    }

    void RaiseButton() {
        spriteRenderer.sprite = upSprite;
    }
}
