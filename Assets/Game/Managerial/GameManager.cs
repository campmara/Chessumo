using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
//using GoogleMobileAds.Api;

public class GameManager : Singleton<GameManager> {
    /////////////////////////////////////////////////////////////////////
    // PUBLICS
    /////////////////////////////////////////////////////////////////////
    public enum State {
        MENU = 0,
        GAME = 1
    }
    public State CurrentState { get { return currentState; } }

    [Header("Game Prefab")]
    public Game gamePrefab;
    [Header("Button Prefabs")]
    public GameObject restartButtonPrefab;
    [Header("UI Prefabs")]
    public GameObject tilePrefab;
    public GameObject scoreEffectPrefab;
    public GameObject nextPieceViewerPrefab;
    [Header("Piece Prefabs")]
    public GameObject kingPrefab;
    public GameObject queenPrefab;
    public GameObject rookPrefab;
    public GameObject bishopPrefab;
    public GameObject knightPrefab;
    public GameObject pawnPrefab;

    [HideInInspector] public ScoreEffect scoreEffect;
    [HideInInspector] public RestartButton restartButton;

    /////////////////////////////////////////////////////////////////////
    // PRIVATES
    /////////////////////////////////////////////////////////////////////

    private Game game;
    private State currentState;

    public void SetVisibility(bool isVisible) {
        UIManager.Instance.SetVisibility(isVisible);

        if (game != null) {
            game.gameObject.SetActive(isVisible);
        } else {
            if (isVisible) {
                UIManager.Instance.IntroduceTopBar(1f);
            }
        }
    }

    void Start() {
        // 60fps will eat battery, but threes does it so w/e.
        Application.targetFrameRate = SaveDataManager.Instance.IsBatterySaverOn() ? 30 : 60;

        // Init DOTween.
        DOTween.Init();

        currentState = State.MENU;

        //AdManager.Instance.OnAdClosed += OnAdClosed;

        // Create the UI first, before introducing to avoid introductory jitter.
        CreateUI();
    }

    void CreateUI() {
        GameObject restartButtonObj = Instantiate(restartButtonPrefab) as GameObject;
        restartButtonObj.name = "Restart Button";
        restartButtonObj.transform.parent = transform;
        restartButton = restartButtonObj.GetComponent(typeof(RestartButton)) as RestartButton;

        GameObject scoreEffectObj = Instantiate(scoreEffectPrefab) as GameObject;
        scoreEffectObj.name = "Score Effect";
        scoreEffectObj.transform.parent = transform;
        scoreEffect = scoreEffectObj.GetComponent<ScoreEffect>();

        if (Screen.safeArea.height < Screen.height) // then we know we're on iphone x
        {
            Camera.main.orthographicSize = 5.25f;
            scoreEffect.SetPosition(Vector3.up * -3.75f);
        }

        UIManager.Instance.Initialize();
    }

    public void OnGameEnd() {
        UIManager.Instance.SubmitFinalScore();
        SaveDataManager.Instance.IncrementTotalGames();
        //AdManager.Instance.TryShowVideoAd();
        GameManager.Instance.BeginGame();
    }

    // should just start a new game instead of hitting restart button.
    public void OnAdClosed() {
        GameManager.Instance.BeginGame();
    }

    public void StartNewGame() {
        if (game != null) {
            UIManager.Instance.OnGameRestarted();

            game.Unload();

            game = null;

            restartButton.SetButtonEnabled(true);
        } else {
            restartButton.SetButtonEnabled(false);
            restartButton.SetReadyForInput(false);

            restartButton.KillPulse();
            restartButton.ResetEffect();

            restartButton.SetButtonEnabled(true);
        }
    }

    public bool IsGameActive() {
        return game != null;
    }

    public void BeginGame() {
        if (game == null) {
            Debug.Log("[GAME MANAGER] Loading and Beginning Game.");

            game = Instantiate(gamePrefab) as Game;
            game.name = "Chessumo Game";
            game.transform.parent = transform;

            game.Load();
        } else {
            Debug.Log("[GAME MANAGER] Unloading Game");

            UIManager.Instance.OnGameRestarted();

            game.Unload();

            game = null;

            // Reload the game, now that we've unloaded everything correctly.
            BeginGame();
        }
    }

    public Vector2 CoordinateToPosition(IntVector2 coordinate) {
        //float xPos = coordinate.x - Mathf.Floor(Constants.I.GRID_SIZE.x / 2f) + Constants.I.GRID_OFFSET_X;
        //float yPos = coordinate.y - Mathf.Floor(Constants.I.GRID_SIZE.y / 2f) + Constants.I.GRID_OFFSET_Y;

        float xPos = (coordinate.x - (Constants.I.GridSize.x / 2f)) + 0.5f + Constants.I.GridOffsetX;
        float yPos = (coordinate.y - (Constants.I.GridSize.y / 2f)) + 0.5f + Constants.I.GridOffsetY;
        return new Vector2(xPos, yPos);
    }

    public void GrowMe(GameObject obj, Ease ease = Ease.OutBack) {
        StartCoroutine(GrowToScale(obj, ease));
    }

    IEnumerator GrowToScale(GameObject obj, Ease ease) {
        // Neat little effect for now to compensate for the fact that shit would just appear out of nowhere otherwise. this will die someday.

        if (!obj) {
            yield break;
        }

        Vector3 startScale = new Vector3(0f, 0f, 1f);
        Vector3 desiredScale = obj.transform.localScale;

        // Scale to 0 first.
        obj.transform.localScale = startScale;

        // Then rescale via tween.
        if (obj) {
            obj.transform.DOScale(desiredScale, 1f)
                .SetEase(ease);
        }

        yield return null;
    }

    public void GrowMeFromSlit(GameObject obj, float delay = 0f, Ease ease = Ease.OutBack) {
        StartCoroutine(GrowFromSlit(obj, delay, ease));
    }

    IEnumerator GrowFromSlit(GameObject obj, float delay, Ease ease) {
        if (!obj) {
            yield break;
        }

        Vector3 startScale = new Vector3(obj.transform.localScale.x, 0f, 1f);
        float desiredScaleY = obj.transform.localScale.y;

        // Scale to 0 first.
        obj.transform.localScale = startScale;

        // Then wait the delay.
        yield return new WaitForSeconds(delay);

        // Then rescale via tween.
        if (obj) {
            obj.transform.DOScaleY(desiredScaleY, 1f)
                .SetEase(ease);
        }

        yield return null;
    }

    public void ShrinkMeToSlit(GameObject obj, float delay = 0f, Ease ease = Ease.OutBack) {
        StartCoroutine(ShrinkToSlit(obj, delay, ease));
    }

    IEnumerator ShrinkToSlit(GameObject obj, float delay, Ease ease) {
        // Then wait the delay.
        yield return new WaitForSeconds(delay);

        // Then rescale via tween.
        if (obj) {
            obj.transform.DOScaleY(0f, 1f)
                .SetEase(ease);
        }

        yield return null;
    }

    public void IntroduceFromSide(GameObject obj, float delay, bool isRight, Ease ease = Ease.OutBounce) {
        StartCoroutine(IntroduceFromSideRoutine(obj, delay, isRight, ease));
    }

    IEnumerator IntroduceFromSideRoutine(GameObject obj, float delay, bool isRight, Ease ease) {
        int sign = isRight ? 1 : -1;
        Vector3 startPos = new Vector3(5f * sign, obj.transform.position.y, obj.transform.position.z);
        Vector3 endPos = obj.transform.position;

        // send off to the side first.
        obj.transform.position = startPos;

        // Then wait the delay.
        yield return new WaitForSeconds(delay);

        // Then rescale via tween.
        if (obj) {
            obj.transform.DOMove(endPos, 1f)
                .SetEase(ease);
        }

        yield return null;
    }
}
















