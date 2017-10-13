using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialAnim1 : MonoBehaviour 
{
    [SerializeField] private GameObject pawn;
    [SerializeField] private Color pawnColor;
    [SerializeField] private Color secondaryPawnColor;

    [SerializeField] private GameObject king;
    [SerializeField] private Color kingColor;

    [SerializeField] private Tile bottomTile;
    [SerializeField] private Tile topTile;

    [SerializeField] private Color disabledTint;

    [SerializeField] private ScoreEffect scoreEffect;

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

        // TOUCH THE SCREEN.
        touch.SetActive(true);

        touch.transform.DOScale(new Vector3(0.5f, 0.5f, 1f), 0.5f);
        Tween tween = (touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(1f, 0.5f);

        yield return tween.WaitForCompletion();

        // TILES REACT
        bottomTile.SetState(TileState.DRAWN, pawnColor);
        topTile.SetState(TileState.POSSIBLE, secondaryPawnColor);
        pawn.transform.position += Vector3.up * 0.1f;
        touch.transform.position += Vector3.up * 0.1f;

        yield return new WaitForSeconds(1f);

        // DRAG FINGER UP
        tween = touch.transform.DOMoveY(king.transform.position.y, 1f);

        yield return tween.WaitForCompletion();

        // TOP TILE REACTS
        topTile.SetState(TileState.DRAWN, pawnColor);
        king.transform.position += Vector3.up * 0.1f;
        touch.transform.position += Vector3.up * 0.1f;

        yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        touch.transform.DOScale(initialTouchScale, 0.5f);
        (touch.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).DOFade(0f, 0.5f);

        pawn.transform.DOMoveY(topTile.transform.position.y, 0.75f);
        tween = king.transform.DOMoveY(king.transform.position.y + 1f, 0.75f);

        yield return tween.WaitForCompletion();

        // LET KING FALL
        (king.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).sortingLayerName = "Falling Pieces";
        tween = king.transform.DOMoveY(-6f, 0.75f).SetEase(Ease.InCubic);

        // DISABLE TOUCH DESIGNATOR AND WAIT
        touch.SetActive(false);
		bottomTile.SetState(TileState.DEFAULT, pawnColor);
		topTile.SetState(TileState.DEFAULT, pawnColor);

        yield return tween.WaitForCompletion();

        scoreEffect.OnOneScored(kingColor);
        (king.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer).sortingLayerName = "Pieces";
        king.transform.position = new Vector3(0f, 20f, 0f);

        yield return new WaitForSeconds(1f);

		king.transform.DOMoveY(topTile.transform.position.y, 0.75f);
        tween = pawn.transform.DOMoveY(bottomTile.transform.position.y, 0.75f);

        yield return tween.WaitForCompletion();

        touch.transform.position = pawn.transform.position;

        StartCoroutine(LoopRoutine());
    }
}
