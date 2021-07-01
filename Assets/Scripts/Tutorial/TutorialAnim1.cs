using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mara.MrTween;

public class TutorialAnim1 : MonoBehaviour {
    [SerializeField] private Image pawn;
    [SerializeField] private Color pawnColor;
    [SerializeField] private Color secondaryPawnColor;

    [SerializeField] private Image king;
    [SerializeField] private Color kingColor;

    [SerializeField] private Image bishop;

    [SerializeField] private Image bottomTile;
    [SerializeField] private Image topTile;

    [SerializeField] private Sprite tileUpSprite;
    [SerializeField] private Sprite tileDownSprite;

    [SerializeField] private Color disabledTint;

    [SerializeField] private Image touch;

    private Vector3 initialTouchScale;
    private Vector2 initialPawnPos;
    private Vector2 initialKingPos;

    void Awake() {
        initialTouchScale = touch.transform.localScale;
        initialPawnPos = pawn.rectTransform.anchoredPosition;
        initialKingPos = king.rectTransform.anchoredPosition;
    }

    void OnEnable() {
        pawn.rectTransform.anchoredPosition = initialPawnPos;
        king.rectTransform.anchoredPosition = initialKingPos;
        king.rectTransform.SetSiblingIndex(3);
        bishop.rectTransform.localScale = Vector3.zero;
        touch.gameObject.SetActive(false);

        bottomTile.sprite = tileDownSprite;
        bottomTile.color = disabledTint;
        topTile.sprite = tileDownSprite;
        topTile.color = disabledTint;

        StartCoroutine(LoopRoutine());
    }

    private IEnumerator LoopRoutine() {
        yield return new WaitForSeconds(1f);

        // TOUCH THE SCREEN.
        touch.rectTransform.anchoredPosition = pawn.rectTransform.anchoredPosition;
        touch.rectTransform.localScale = new Vector3(4f, 4f, 1f);
        touch.color = new Color(touch.color.r, touch.color.g, touch.color.b, 0f);
        touch.gameObject.SetActive(true);

        touch.rectTransform.LocalScaleTo(new Vector3(1f, 1f, 1f), 0.5f).Start();
        ITween<float> touchTween = touch.AlphaTo(1f, 0.5f);
        touchTween.Start();
        yield return touchTween.WaitForCompletion();

        // TILES REACT
        bottomTile.sprite = tileUpSprite;
        bottomTile.color = pawnColor;
        topTile.color = secondaryPawnColor;
        pawn.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // DRAG FINGER UP
        ITween<Vector2> anchorTween = touch.rectTransform.YAnchoredPositionTo(king.rectTransform.anchoredPosition.y, 1f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        // TOP TILE REACTS
        topTile.sprite = tileUpSprite;
        topTile.color = pawnColor;
        king.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        touch.transform.LocalScaleTo(initialTouchScale, 0.5f).Start();
        touch.AlphaTo(0f, 0.5f).Start();

        pawn.rectTransform.YAnchoredPositionTo(initialKingPos.y, 0.75f).Start();
        anchorTween = king.rectTransform.YAnchoredPositionTo(king.rectTransform.anchoredPosition.y + 125f, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        // LET KING FALL
        king.transform.SetAsFirstSibling();
        anchorTween = king.rectTransform.YAnchoredPositionTo(-1000f, 0.75f).SetEaseType(EaseType.CubicIn);
        anchorTween.Start();

        // DISABLE TOUCH DESIGNATOR AND WAIT
        touch.gameObject.SetActive(false);
        bottomTile.sprite = tileDownSprite;
        bottomTile.color = disabledTint;
        topTile.sprite = tileDownSprite;
        topTile.color = disabledTint;

        yield return anchorTween.WaitForCompletion();

        GameManager.Instance.scoreEffect.OnOneScored(kingColor);
        bishop.rectTransform.LocalScaleTo(Vector3.one, 0.75f).Start();
        king.rectTransform.SetSiblingIndex(3);
        king.rectTransform.anchoredPosition = new Vector2(0f, 1000f);

        yield return new WaitForSeconds(2f);

        king.rectTransform.YAnchoredPositionTo(initialKingPos.y, 0.75f).Start();
        bishop.rectTransform.LocalScaleTo(Vector3.zero, 0.75f).Start();
        anchorTween = pawn.rectTransform.YAnchoredPositionTo(initialPawnPos.y, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        touch.rectTransform.anchoredPosition = pawn.rectTransform.anchoredPosition;

        StartCoroutine(LoopRoutine());
    }
}
