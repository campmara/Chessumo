using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mara.MrTween;

public class TutorialAnim3 : MonoBehaviour {
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

    void Awake() {
        initialTouchScale = touch.transform.localScale;
        initialRookPos = rook.rectTransform.anchoredPosition;
        initialBishopPos = bishop.rectTransform.anchoredPosition;
        initialKingPos = king.rectTransform.anchoredPosition;
        initialQueenPos = queen.rectTransform.anchoredPosition;
    }

    void OnEnable() {
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

    private IEnumerator LoopRoutine() {
        yield return new WaitForSeconds(1f);

        // ===========
        // FIRST PHASE
        // ===========

        // TOUCH THE SCREEN.
        touch.rectTransform.anchoredPosition = rook.rectTransform.anchoredPosition;
        touch.rectTransform.localScale = new Vector3(4f, 4f, 1f);
        touch.color = new Color(touch.color.r, touch.color.g, touch.color.b, 0f);
        touch.gameObject.SetActive(true);

        touch.rectTransform.LocalScaleTo(new Vector3(1f, 1f, 1f), 0.5f).Start();
        ITween<float> floatTween = touch.AlphaTo(1f, 0.5f);
        floatTween.Start();
        yield return floatTween.WaitForCompletion();

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
        ITween<Vector2> anchorTween = touch.rectTransform.XAnchoredPositionTo(bishop.rectTransform.anchoredPosition.x, 0.5f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();
        tileB.sprite = tileUpSprite;
        tileB.color = rookColor;
        bishop.rectTransform.anchoredPosition += Vector2.up * 10f;

        anchorTween = touch.rectTransform.XAnchoredPositionTo(king.rectTransform.anchoredPosition.x, 0.5f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();
        tileC.sprite = tileUpSprite;
        tileC.color = rookColor;
        king.rectTransform.anchoredPosition += Vector2.up * 10f;

        anchorTween = touch.rectTransform.XAnchoredPositionTo(queen.rectTransform.anchoredPosition.x, 0.5f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();
        tileD.sprite = tileUpSprite;
        tileD.color = rookColor;
        queen.rectTransform.anchoredPosition += Vector2.up * 10f;

        yield return new WaitForSeconds(1f);

        // LET GO FINGER AND PIECES MOVE
        touch.transform.LocalScaleTo(initialTouchScale, 0.5f).Start();
        touch.AlphaTo(0f, 0.5f).Start();

        // QUEEN GETS KNOCKED
        rook.rectTransform.XAnchoredPositionTo(bishop.rectTransform.anchoredPosition.x, 0.75f).Start();
        bishop.rectTransform.XAnchoredPositionTo(king.rectTransform.anchoredPosition.x, 0.75f).Start();
        king.rectTransform.XAnchoredPositionTo(queen.rectTransform.anchoredPosition.x, 0.75f).Start();
        anchorTween = queen.rectTransform.XAnchoredPositionTo(queen.rectTransform.anchoredPosition.x + 125f, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();

        queen.rectTransform.YAnchoredPositionTo(-1000f, 0.75f)
            .SetEaseType(EaseType.CubicIn)
            .SetCompletionHandler((_) => GameManager.Instance.scoreEffect.OnOneScored(queenColor))
            .Start();

        // KING GETS KNOCKED
        rook.rectTransform.XAnchoredPositionTo(bishop.rectTransform.anchoredPosition.x, 0.75f).Start();
        bishop.rectTransform.XAnchoredPositionTo(king.rectTransform.anchoredPosition.x, 0.75f).Start();
        anchorTween = king.rectTransform.XAnchoredPositionTo(king.rectTransform.anchoredPosition.x + 125f, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();
        king.rectTransform.YAnchoredPositionTo(-1000f, 0.75f)
            .SetEaseType(EaseType.CubicIn)
            .SetCompletionHandler((_) => GameManager.Instance.scoreEffect.OnTwoScored(kingColor))
            .Start();

        // BISHOP GETS KNOCKED
        rook.rectTransform.XAnchoredPositionTo(bishop.rectTransform.anchoredPosition.x, 0.75f).Start();
        anchorTween = bishop.rectTransform.XAnchoredPositionTo(bishop.rectTransform.anchoredPosition.x + 125f, 0.75f);
        anchorTween.Start();
        yield return anchorTween.WaitForCompletion();
        bishop.rectTransform.YAnchoredPositionTo(-1000f, 0.75f)
            .SetEaseType(EaseType.CubicIn)
            .SetCompletionHandler((_) => GameManager.Instance.scoreEffect.OnThreeScored(bishopColor))
            .Start();

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

        rook.rectTransform.XAnchoredPositionTo(initialRookPos.x, 1f)
            .SetCompletionHandler((_) => rook.color = Color.white)
            .Start();
        yield return new WaitForSeconds(0.25f);
        queen.rectTransform.YAnchoredPositionTo(initialQueenPos.y, 1f).Start();
        yield return new WaitForSeconds(0.25f);
        king.rectTransform.YAnchoredPositionTo(initialKingPos.y, 1f).Start();
        yield return new WaitForSeconds(0.25f);
        bishop.rectTransform.YAnchoredPositionTo(initialBishopPos.y, 1f).Start();

        yield return new WaitForSeconds(2f);

        touch.rectTransform.anchoredPosition = rook.rectTransform.anchoredPosition;

        StartCoroutine(LoopRoutine());
    }
}
