using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TutorialAnim3 : MonoBehaviour 
{
	[SerializeField] private Image rook;
	[SerializeField] private Color rookColor;
	[SerializeField] private Color secondaryRookColor;

	[SerializeField] private Image bishop;
    [SerializeField] private Color bishopColor;

    [SerializeField] private Image king;
    [SerializeField] private Color kingColor;

    [SerializeField] private Image queen;
    [SerializeField] private Color queenColor;

	[SerializeField] private Color disabledTint;

	[SerializeField] private Image tileA;
	[SerializeField] private Image tileB;
	[SerializeField] private Image tileC;
    [SerializeField] private Image tileD;

	[SerializeField] private Sprite tileUpSprite;
    [SerializeField] private Sprite tileDownSprite;

	[SerializeField] private Image touch;

	private Vector3 initialTouchScale;
	private Vector2 initialRookPos;
	private Vector2 initialBishopPos;
    private Vector2 initialKingPos;
	private Vector2 initialQueenPos;

	void Awake()
	{
		initialTouchScale = touch.transform.localScale;
		initialRookPos = rook.rectTransform.anchoredPosition;
        initialBishopPos = bishop.rectTransform.anchoredPosition;
        initialKingPos = king.rectTransform.anchoredPosition;
		initialQueenPos = queen.rectTransform.anchoredPosition;
	}

	void OnEnable()
	{
		rook.rectTransform.anchoredPosition = initialRookPos;
		bishop.rectTransform.anchoredPosition = initialBishopPos;
		king.rectTransform.anchoredPosition = initialKingPos;
		queen.rectTransform.anchoredPosition = initialQueenPos;
		touch.gameObject.SetActive(false);

		tileA.sprite = tileDownSprite;
		tileA.color = disabledTint;
		tileB.sprite = tileDownSprite;
		tileB.color = disabledTint;
		tileC.sprite = tileDownSprite;
		tileC.color = disabledTint;
		tileD.sprite = tileDownSprite;
		tileD.color = disabledTint;

		StartCoroutine(LoopRoutine());
	}

	private IEnumerator LoopRoutine()
	{
		yield return new WaitForSeconds(1f);

		// ===========
		// FIRST PHASE
		// ===========

		// TOUCH THE SCREEN.
		touch.rectTransform.anchoredPosition = rook.rectTransform.anchoredPosition;
        touch.rectTransform.localScale = new Vector3(4f, 4f, 1f);
        touch.color = new Color(touch.color.r, touch.color.g, touch.color.b, 0f);
		touch.gameObject.SetActive(true);

		touch.rectTransform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
        Tween tween = touch.DOFade(1f, 0.5f);

		yield return tween.WaitForCompletion();

		// TILES REACT
		tileA.sprite = tileUpSprite;
        tileA.color = rookColor;
        tileB.color = secondaryRookColor;
		tileC.color = secondaryRookColor;
		tileD.color = secondaryRookColor;
        rook.rectTransform.anchoredPosition += Vector2.up * 10f;
        touch.rectTransform.anchoredPosition += Vector2.up * 10f;

		yield return new WaitForSeconds(1f);

		// DRAG FINGER RIGHT, TILES REACTING ALONG THE WAY
        tween = touch.rectTransform.DOAnchorPosX(bishop.rectTransform.anchoredPosition.x, 0.5f);
		yield return tween.WaitForCompletion();
		tileB.sprite = tileUpSprite;
		tileB.color = rookColor;
        bishop.rectTransform.anchoredPosition += Vector2.up * 10f;

		tween = touch.rectTransform.DOAnchorPosX(king.rectTransform.anchoredPosition.x, 0.5f);
		yield return tween.WaitForCompletion();
		tileC.sprite = tileUpSprite;
		tileC.color = rookColor;
		king.rectTransform.anchoredPosition += Vector2.up * 10f;

        tween = touch.rectTransform.DOAnchorPosX(queen.rectTransform.anchoredPosition.x, 0.5f);
		yield return tween.WaitForCompletion();
		tileD.sprite = tileUpSprite;
		tileD.color = rookColor;
        queen.rectTransform.anchoredPosition += Vector2.up * 10f;

		yield return new WaitForSeconds(1f);

		// LET GO FINGER AND PIECES MOVE
		touch.transform.DOScale(initialTouchScale, 0.5f);
        touch.DOFade(0f, 0.5f);

        // QUEEN GETS KNOCKED
        rook.rectTransform.DOAnchorPosX(bishop.rectTransform.anchoredPosition.x, 0.75f);
        bishop.rectTransform.DOAnchorPosX(king.rectTransform.anchoredPosition.x, 0.75f);
        king.rectTransform.DOAnchorPosX(queen.rectTransform.anchoredPosition.x, 0.75f);
        tween = queen.rectTransform.DOAnchorPosX(queen.rectTransform.anchoredPosition.x + 125f, 0.75f);
		yield return tween.WaitForCompletion();
        queen.rectTransform.DOAnchorPosY(-1000f, 0.75f).SetEase(Ease.InCubic).OnComplete(() => GameManager.Instance.scoreEffect.OnOneScored(queenColor));

		// KING GETS KNOCKED
		rook.rectTransform.DOAnchorPosX(bishop.rectTransform.anchoredPosition.x, 0.75f);
		bishop.rectTransform.DOAnchorPosX(king.rectTransform.anchoredPosition.x, 0.75f);
		tween = king.rectTransform.DOAnchorPosX(king.rectTransform.anchoredPosition.x + 125f, 0.75f);
		yield return tween.WaitForCompletion();
		king.rectTransform.DOAnchorPosY(-1000f, 0.75f).SetEase(Ease.InCubic).OnComplete(() => GameManager.Instance.scoreEffect.OnTwoScored(kingColor));

		// BISHOP GETS KNOCKED
		rook.rectTransform.DOAnchorPosX(bishop.rectTransform.anchoredPosition.x, 0.75f);
        tween = bishop.rectTransform.DOAnchorPosX(bishop.rectTransform.anchoredPosition.x + 125f, 0.75f);
		yield return tween.WaitForCompletion();
        bishop.rectTransform.DOAnchorPosY(-1000f, 0.75f).SetEase(Ease.InCubic).OnComplete(() => GameManager.Instance.scoreEffect.OnThreeScored(bishopColor));

		// DISABLE TOUCH DESIGNATOR AND WAIT
		rook.color = disabledTint; // disable rook
        rook.rectTransform.anchoredPosition += Vector2.down * 10f;
		touch.gameObject.SetActive(false);
		tileA.sprite = tileDownSprite;
		tileA.color = disabledTint;
		tileB.sprite = tileDownSprite;
		tileB.color = disabledTint;
		tileC.sprite = tileDownSprite;
		tileC.color = disabledTint;
		tileD.sprite = tileDownSprite;
		tileD.color = disabledTint;

		yield return new WaitForSeconds(1f);

        // RESET THE BOARD
        bishop.rectTransform.anchoredPosition = new Vector2(initialBishopPos.x, 1000f);
        king.rectTransform.anchoredPosition = new Vector2(initialKingPos.x, 1000f);
        queen.rectTransform.anchoredPosition = new Vector2(initialQueenPos.x, 1000f);

        rook.rectTransform.DOAnchorPosX(initialRookPos.x, 1f).OnComplete(() => rook.color = Color.white);
        yield return new WaitForSeconds(0.25f);
        queen.rectTransform.DOAnchorPosY(initialQueenPos.y, 1f);
		yield return new WaitForSeconds(0.25f);
		king.rectTransform.DOAnchorPosY(initialKingPos.y, 1f);
		yield return new WaitForSeconds(0.25f);
        bishop.rectTransform.DOAnchorPosY(initialBishopPos.y, 1f);

        yield return new WaitForSeconds(2f);

        touch.rectTransform.anchoredPosition = rook.rectTransform.anchoredPosition;

		StartCoroutine(LoopRoutine());
	}
}
