using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialAnim3 : MonoBehaviour 
{
	[SerializeField] private GameObject rook;
	[SerializeField] private Color rookColor;
	[SerializeField] private Color secondaryRookColor;

	[SerializeField] private GameObject bishop;
    [SerializeField] private Color bishopColor;

    [SerializeField] private GameObject king;
    [SerializeField] private Color kingColor;

    [SerializeField] private GameObject queen;
    [SerializeField] private Color queenColor;

	[SerializeField] private Color disabledTint;

	[SerializeField] private Tile tileA;
	[SerializeField] private Tile tileB;
	[SerializeField] private Tile tileC;
    [SerializeField] private Tile tileD;

    [SerializeField] private ScoreEffect scoreEffect;

	[SerializeField] private GameObject touch;
	private Vector3 initialTouchScale;

	void Awake()
	{
		initialTouchScale = touch.transform.localScale;
		StartCoroutine(LoopRoutine());
	}

	private IEnumerator LoopRoutine()
	{
		yield return new WaitForSeconds(1f);

		// ===========
		// FIRST PHASE
		// ===========

		// TOUCH THE SCREEN.
		touch.SetActive(true);

		touch.transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0.5f);
		Tween tween = (touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(1f, 0.5f);

		yield return tween.WaitForCompletion();

		// TILES REACT
        tileA.SetState(TileState.DRAWN, rookColor);
        tileB.SetState(TileState.POSSIBLE, secondaryRookColor);
        tileC.SetState(TileState.POSSIBLE, secondaryRookColor);
        tileD.SetState(TileState.POSSIBLE, secondaryRookColor);
        rook.transform.position += Vector3.up * 0.1f;
		touch.transform.position += Vector3.up * 0.1f;

		yield return new WaitForSeconds(1f);

		// DRAG FINGER RIGHT, TILES REACTING ALONG THE WAY
        tween = touch.transform.DOMoveX(bishop.transform.position.x, 0.5f);
		yield return tween.WaitForCompletion();
        tileB.SetState(TileState.DRAWN, rookColor);
        bishop.transform.position += Vector3.up * 0.1f;

		tween = touch.transform.DOMoveX(king.transform.position.x, 0.5f);
		yield return tween.WaitForCompletion();
		tileC.SetState(TileState.DRAWN, rookColor);
		king.transform.position += Vector3.up * 0.1f;

        tween = touch.transform.DOMoveX(queen.transform.position.x, 0.5f);
		yield return tween.WaitForCompletion();
		tileD.SetState(TileState.DRAWN, rookColor);
        queen.transform.position += Vector3.up * 0.1f;

		yield return new WaitForSeconds(1f);

		// LET GO FINGER AND PIECES MOVE
		touch.transform.DOScale(initialTouchScale, 0.5f);
		(touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(0f, 0.5f);

        // QUEEN GETS KNOCKED
        rook.transform.DOMoveX(bishop.transform.position.x, 0.75f);
        bishop.transform.DOMoveX(king.transform.position.x, 0.75f);
        king.transform.DOMoveX(queen.transform.position.x, 0.75f);
        tween = queen.transform.DOMoveX(queen.transform.position.x + 1f, 0.75f);
		yield return tween.WaitForCompletion();
        queen.transform.DOMoveY(-6f, 0.75f).SetEase(Ease.InCubic).OnComplete(() => scoreEffect.OnOneScored(queenColor));

		// KING GETS KNOCKED
		rook.transform.DOMoveX(bishop.transform.position.x, 0.75f);
		bishop.transform.DOMoveX(king.transform.position.x, 0.75f);
		tween = king.transform.DOMoveX(king.transform.position.x + 1f, 0.75f);
		yield return tween.WaitForCompletion();
		king.transform.DOMoveY(-6f, 0.75f).SetEase(Ease.InCubic).OnComplete(() => scoreEffect.OnTwoScored(kingColor));

		// BISHOP GETS KNOCKED
		rook.transform.DOMoveX(bishop.transform.position.x, 0.75f);
        tween = bishop.transform.DOMoveX(bishop.transform.position.x + 1f, 0.75f);
		yield return tween.WaitForCompletion();
        bishop.transform.DOMoveY(-6f, 0.75f).SetEase(Ease.InCubic).OnComplete(() => scoreEffect.OnThreeScored(bishopColor));

		// DISABLE TOUCH DESIGNATOR AND WAIT
		(rook.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color = disabledTint; // disable rook
        rook.transform.position += Vector3.down * 0.1f;
		touch.SetActive(false);
        tileA.SetState(TileState.DEFAULT, rookColor);
        tileB.SetState(TileState.DEFAULT, rookColor);
        tileC.SetState(TileState.DEFAULT, rookColor);
        tileD.SetState(TileState.DEFAULT, rookColor);

		yield return new WaitForSeconds(1f);

        // RESET THE BOARD
        bishop.transform.position = new Vector3(tileB.transform.position.x, 20f, 0f);
        king.transform.position = new Vector3(tileC.transform.position.x, 20f, 0f);
        queen.transform.position = new Vector3(tileD.transform.position.x, 20f, 0f);

        rook.transform.DOMoveX(tileA.transform.position.x, 1f).OnComplete(() => (rook.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color = Color.white);
        yield return new WaitForSeconds(0.25f);
        queen.transform.DOMoveY(tileB.transform.position.y, 1f);
		yield return new WaitForSeconds(0.25f);
		king.transform.DOMoveY(tileC.transform.position.y, 1f);
		yield return new WaitForSeconds(0.25f);
        bishop.transform.DOMoveY(tileD.transform.position.y, 1f);

        yield return new WaitForSeconds(2f);

        touch.transform.position = rook.transform.position;

		StartCoroutine(LoopRoutine());
	}
}
