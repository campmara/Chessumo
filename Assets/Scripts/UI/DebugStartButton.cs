using UnityEngine;
using Mara.MrTween;

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
        transform.YPositionTo(Constants.I.StartButtonRaisedY, Constants.I.StartButtonTweenTime)
            .SetEaseType(EaseType.BackOut)
            .Start();

        Invoke("RaiseButton", Constants.I.StartButtonTweenTime + 0.25f);
    }

    public void Lower() {
        transform.YPositionTo(Constants.I.StartButtonLoweredY, Constants.I.StartButtonTweenTime)
            .SetEaseType(EaseType.QuintIn)
            .Start();
    }

    void RaiseButton() {
        spriteRenderer.sprite = upSprite;
    }
}
