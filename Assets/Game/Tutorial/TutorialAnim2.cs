using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialAnim2 : MonoBehaviour 
{
	[SerializeField] private GameObject pawn;
	[SerializeField] private Color pawnColor;
	[SerializeField] private Color secondaryPawnColor;

	[SerializeField] private GameObject king;
	[SerializeField] private Color kingColor;
	[SerializeField] private Color secondaryKingColor;

    [SerializeField] private Color disabledTint;

	[SerializeField] private Tile bottomTile;
    [SerializeField] private Tile middleTile;
	[SerializeField] private Tile topTile;

	[SerializeField] private GameObject touch;
	private Vector3 initialTouchScale;

	void OnEnable()
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
		bottomTile.SetState(TileState.DRAWN, pawnColor);
        middleTile.SetState(TileState.POSSIBLE, secondaryPawnColor);
		pawn.transform.position += Vector3.up * 0.1f;
		touch.transform.position += Vector3.up * 0.1f;

		yield return new WaitForSeconds(1f);

		// DRAG FINGER UP
		tween = touch.transform.DOMoveY(king.transform.position.y, 1f);

		yield return tween.WaitForCompletion();

		// MIDDLE TILE REACTS
        middleTile.SetState(TileState.DRAWN, pawnColor);
		king.transform.position += Vector3.up * 0.1f;
		touch.transform.position += Vector3.up * 0.1f;

		yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        (king.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color = Color.white; // enable king

		touch.transform.DOScale(initialTouchScale, 0.5f);
		(touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(0f, 0.5f);

		pawn.transform.DOMoveY(middleTile.transform.position.y, 0.75f);
		tween = king.transform.DOMoveY(topTile.transform.position.y, 0.75f);

		yield return tween.WaitForCompletion();

		// DISABLE TOUCH DESIGNATOR AND WAIT
        (pawn.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color = disabledTint; // disable pawn
		touch.SetActive(false);
		bottomTile.SetState(TileState.DEFAULT, pawnColor);
        middleTile.SetState(TileState.DEFAULT, pawnColor);

		yield return new WaitForSeconds(1f);

		// ============
		// SECOND PHASE
		// ============

		// TOUCH THE SCREEN.
		touch.SetActive(true);
        touch.transform.position = king.transform.position;

		touch.transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0.5f);
		tween = (touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(1f, 0.5f);

		yield return tween.WaitForCompletion();

		// TILES REACT
        topTile.SetState(TileState.DRAWN, kingColor);
        middleTile.SetState(TileState.POSSIBLE, secondaryKingColor);
		king.transform.position += Vector3.up * 0.1f;
		touch.transform.position += Vector3.up * 0.1f;

		yield return new WaitForSeconds(1f);

		// DRAG FINGER DOWN
		tween = touch.transform.DOMoveY(pawn.transform.position.y, 1f);

		yield return tween.WaitForCompletion();

		// MIDDLE TILE REACTS
        middleTile.SetState(TileState.DRAWN, kingColor);
		pawn.transform.position += Vector3.up * 0.1f;
		touch.transform.position += Vector3.up * 0.1f;

		yield return new WaitForSeconds(1f);

		// LET GO FINGER AND PIECES MOVE
        (pawn.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color = Color.white; // enable pawn

		touch.transform.DOScale(initialTouchScale, 0.5f);
		(touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(0f, 0.5f);

		pawn.transform.DOMoveY(bottomTile.transform.position.y, 0.75f);
		tween = king.transform.DOMoveY(middleTile.transform.position.y, 0.75f);

		yield return tween.WaitForCompletion();

		// DISABLE TOUCH DESIGNATOR AND WAIT
        (king.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).color = disabledTint; // disable king
		touch.SetActive(false);
        middleTile.SetState(TileState.DEFAULT, pawnColor);
		topTile.SetState(TileState.DEFAULT, pawnColor);
		touch.transform.position = pawn.transform.position;

		StartCoroutine(LoopRoutine());
	}
}
