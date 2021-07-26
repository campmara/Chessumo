using UnityEngine;
using Mara.MrTween;

public class NextPieceViewer : MonoBehaviour {
    [SerializeField] SpriteRenderer sprite;

    public void OnEnable() {
        transform.position = new Vector3(transform.position.x, 15f, transform.position.z);
        transform.YPositionTo(1.5f, 0.8f)
            .SetEaseType(EaseType.Linear)
            .SetDelay(1.5f)
            .SetStartHandler((_) => AudioManager.Instance.PlayNPVEnter())
            .Start();
    }

    public void PositionAlongTop(Vector2Int coords) {
        Vector2 pos = GameManager.Instance.CoordinateToPosition(coords);
        transform.XPositionTo(pos.x, 0.75f).SetEaseType(EaseType.QuintInOut).Start();
    }

    public void FadeOut() {
        sprite.AlphaTo(0f, 0.75f).Start();
    }

    public void ShowPiece(int pieceID) {
        Color viewerColor = GameManager.Instance.piecePrefabs[pieceID].GetComponent<Piece>().FullColor;
        viewerColor.a = 0.25f;
        sprite.ColorTo(viewerColor, 0.75f).Start();
    }
}
