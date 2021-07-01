using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mara.MrTween;

public class TutorialAnim2 : MonoBehaviour {
    [SerializeField] private Image pawn;
    [SerializeField] private Color pawnColor;
    [SerializeField] private Color secondaryPawnColor;

    [SerializeField] private Image king;
    [SerializeField] private Color kingColor;
    [SerializeField] private Color secondaryKingColor;

    [SerializeField] private Color disabledTint;

    [SerializeField] private Image bottomTile;
    [SerializeField] private Image middleTile;
    [SerializeField] private Image topTile;

    [SerializeField] private Sprite tileUpSprite;
    [SerializeField] private Sprite tileDownSprite;

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
        touch.gameObject.SetActive(false);

        topTile.sprite = tileDownSprite;
        topTile.color = disabledTint;
        middleTile.sprite = tileDownSprite;
        middleTile.color = disabledTint;
        bottomTile.sprite = tileDownSprite;
        bottomTile.color = disabledTint;

        StartCoroutine(LoopRoutine());
    }

    private IEnumerator LoopRoutine() {
        yield return new WaitForSeconds(1f);

        // ===========
        // FIRST PHASE
        // ===========

        // TOUCH THE SCREEN.
        touch.rectTransform.anchoredPosition = pawn.rectTransform.anchoredPosition;
        touch.rectTransform.localScale = new Vector3(4f, 4f, 1f);
        touch.color = new Color(touch.color.r, touch.color.g, touch.color.b, 0f);
        touch.gameObject.SetActive(true);

        touch.rectTransform.LocalScaleTo(new Vector3(1f, 1f, 1f), 0.5f).Start();
        ITween<float> floatTween = touch.AlphaTo(1f, 0.5f);
        floatTween.Start();
        yield return floatTween.WaitForCompletion();

        // TILES REACT
        bottomTile.sprite = tileUpSprite;
        bottomTile.color = pawnColor;
        middleTile.color = secondaryPawnColor;
        pawn.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // DRAG FINGER UP
        ITween<Vector2> anchorTween = touch.rectTransform.YAnchoredPositionTo(king.rectTransform.anchoredPosition.y, 1f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        // MIDDLE TILE REACTS
        middleTile.sprite = tileUpSprite;
        middleTile.color = pawnColor;
        king.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        king.color = Color.white; // enable king

        touch.transform.LocalScaleTo(initialTouchScale, 0.5f).Start();
        touch.AlphaTo(0f, 0.5f).Start();

        pawn.rectTransform.YAnchoredPositionTo(initialKingPos.y, 0.75f).Start();
        anchorTween = king.rectTransform.YAnchoredPositionTo(topTile.rectTransform.anchoredPosition.y - 5f, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        // DISABLE TOUCH DESIGNATOR AND WAIT
        pawn.color = disabledTint; // disable pawn
        touch.gameObject.SetActive(false);
        bottomTile.sprite = tileDownSprite;
        bottomTile.color = disabledTint;
        middleTile.sprite = tileDownSprite;
        middleTile.color = disabledTint;

        yield return new WaitForSeconds(1f);

        // ============
        // SECOND PHASE
        // ============

        // TOUCH THE SCREEN.
        touch.gameObject.SetActive(true);
        touch.rectTransform.anchoredPosition = king.rectTransform.anchoredPosition;

        touch.rectTransform.LocalScaleTo(new Vector3(1f, 1f, 1f), 0.5f).Start();
        floatTween = touch.AlphaTo(1f, 0.5f);
        floatTween.Start();
        yield return floatTween.WaitForCompletion();

        // TILES REACT
        topTile.sprite = tileUpSprite;
        topTile.color = kingColor;
        middleTile.color = secondaryKingColor;
        king.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // DRAG FINGER DOWN
        anchorTween = touch.rectTransform.YAnchoredPositionTo(pawn.rectTransform.anchoredPosition.y, 1f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        // MIDDLE TILE REACTS
        middleTile.sprite = tileUpSprite;
        middleTile.color = kingColor;
        pawn.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        pawn.color = Color.white; // enable pawn

        touch.transform.LocalScaleTo(initialTouchScale, 0.5f).Start();
        touch.AlphaTo(0f, 0.5f).Start();

        pawn.rectTransform.YAnchoredPositionTo(initialPawnPos.y, 0.75f).Start();
        anchorTween = king.rectTransform.YAnchoredPositionTo(initialKingPos.y - 5f, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        // DISABLE TOUCH DESIGNATOR AND WAIT
        king.color = disabledTint; // disable king
        touch.gameObject.SetActive(false);
        topTile.sprite = tileDownSprite;
        topTile.color = disabledTint;
        middleTile.sprite = tileDownSprite;
        middleTile.color = disabledTint;
        touch.rectTransform.anchoredPosition = pawn.rectTransform.anchoredPosition;

        StartCoroutine(LoopRoutine());
    }
}
