using UnityEngine;
using Mara.MrTween;

public class NextPieceViewer : MonoBehaviour {
    [SerializeField] SpriteRenderer sprite;

    [SerializeField] Color kingColor;
    [SerializeField] Color queenColor;
    [SerializeField] Color rookColor;
    [SerializeField] Color bishopColor;
    [SerializeField] Color knightColor;
    [SerializeField] Color pawnColor;

    public void OnEnable() {
        transform.position = new Vector3(transform.position.x, 15f, transform.position.z);
        transform.YPositionTo(1.5f, 0.8f)
            .SetEaseType(EaseType.Linear)
            .SetDelay(1.5f)
            .SetStartHandler((_) => AudioManager.Instance.PlayNPVEnter())
            .Start();
    }

    public void PositionAlongTop(IntVector2 coords) {
        Vector2 pos = GameManager.Instance.CoordinateToPosition(coords);
        transform.XPositionTo(pos.x, 0.75f).SetEaseType(EaseType.QuintInOut).Start();
    }

    public void FadeOut() {
        sprite.AlphaTo(0f, 0.75f).Start();
    }

    public void ShowKing() {
        sprite.ColorTo(kingColor, 0.75f).Start();
    }

    public void ShowQueen() {
        sprite.ColorTo(queenColor, 0.75f).Start();
    }

    public void ShowRook() {
        sprite.ColorTo(rookColor, 0.75f).Start();
    }

    public void ShowBishop() {
        sprite.ColorTo(bishopColor, 0.75f).Start();
    }

    public void ShowKnight() {
        sprite.ColorTo(knightColor, 0.75f).Start();
    }

    public void ShowPawn() {
        sprite.ColorTo(pawnColor, 0.75f).Start();
    }
}
