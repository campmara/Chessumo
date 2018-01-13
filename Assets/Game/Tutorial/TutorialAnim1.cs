using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialAnim1 : MonoBehaviour 
{
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

    void Awake()
    {
        initialTouchScale = touch.transform.localScale;
        initialPawnPos = pawn.rectTransform.anchoredPosition;
        initialKingPos = king.rectTransform.anchoredPosition;
    }

    void OnEnable()
    {
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

    private IEnumerator LoopRoutine()
    {
        yield return new WaitForSeconds(1f);

        // TOUCH THE SCREEN.
        touch.rectTransform.anchoredPosition = pawn.rectTransform.anchoredPosition;
        touch.rectTransform.localScale = new Vector3(4f, 4f, 1f);
        touch.color = new Color(touch.color.r, touch.color.g, touch.color.b, 0f);
        touch.gameObject.SetActive(true);

        touch.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        Tween tween = touch.DOFade(1f, 0.5f);

        yield return tween.WaitForCompletion();

        // TILES REACT
        bottomTile.sprite = tileUpSprite;
        bottomTile.color = pawnColor;
        topTile.color = secondaryPawnColor;
        pawn.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // DRAG FINGER UP
        tween = touch.rectTransform.DOAnchorPosY(king.rectTransform.anchoredPosition.y, 1f);

        yield return tween.WaitForCompletion();

        // TOP TILE REACTS
        topTile.sprite = tileUpSprite;
        topTile.color = pawnColor;
        king.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        touch.transform.DOScale(initialTouchScale, 0.5f);
        touch.DOFade(0f, 0.5f);

        pawn.rectTransform.DOAnchorPosY(initialKingPos.y, 0.75f);
        tween = king.rectTransform.DOAnchorPosY(king.rectTransform.anchoredPosition.y + 125f, 0.75f);

        yield return tween.WaitForCompletion();

        // LET KING FALL
        king.transform.SetAsFirstSibling();
        tween = king.rectTransform.DOAnchorPosY(-1000f, 0.75f).SetEase(Ease.InCubic);

        // DISABLE TOUCH DESIGNATOR AND WAIT
        touch.gameObject.SetActive(false);
        bottomTile.sprite = tileDownSprite;
        bottomTile.color = disabledTint;
        topTile.sprite = tileDownSprite;
        topTile.color = disabledTint;

        yield return tween.WaitForCompletion();

        GameManager.Instance.scoreEffect.OnOneScored(kingColor);
        bishop.rectTransform.DOScale(Vector3.one, 0.75f);
        king.rectTransform.SetSiblingIndex(3);
        king.rectTransform.anchoredPosition = new Vector2(0f, 1000f);

        yield return new WaitForSeconds(2f);

		king.rectTransform.DOAnchorPosY(initialKingPos.y, 0.75f);
        bishop.rectTransform.DOScale(Vector3.zero, 0.75f);
        tween = pawn.rectTransform.DOAnchorPosY(initialPawnPos.y, 0.75f);

        yield return tween.WaitForCompletion();

        touch.rectTransform.anchoredPosition = pawn.rectTransform.anchoredPosition;

        StartCoroutine(LoopRoutine());
    }
}
