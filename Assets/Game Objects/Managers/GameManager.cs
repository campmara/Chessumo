﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class GameManager : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// PUBLICS
	/////////////////////////////////////////////////////////////////////

	public static GameManager Instance = null;

	public enum State
	{
		MENU = 0,
		GAME = 1
	}
	public State CurrentState { get { return currentState; } }

	[Header("Game Prefab")]
	public Game gamePrefab;
	[Header("Button Prefabs")]
	public GameObject startButtonPrefab;
	public GameObject scoresButtonPrefab;
	public GameObject resetButtonPrefab;
	[Header("UI Prefabs")]
	public GameObject tilePrefab;
	public GameObject topUIBarPrefab;
	public GameObject scorePrefab;
	public GameObject scoreEffectPrefab;
	public GameObject highScorePrefab;
	public GameObject nextPieceViewerPrefab;
	[Header("Piece Prefabs")]
	public GameObject kingPrefab;
	public GameObject queenPrefab;
	public GameObject rookPrefab;
	public GameObject bishopPrefab;
	public GameObject knightPrefab;
	public GameObject pawnPrefab;

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////

	private Game game;
	private State currentState;

	private DebugStartButton startButton;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		// This will eat battery, but threes does it so w/e.
		// Eventually, adding a "conserve battery" option that sets this to 30 would be good.
		Application.targetFrameRate = 60;

		// Init DOTween.
		DOTween.Init();

		currentState = State.MENU;

		// Create the start button 1 second in. Just to make sure we don't jitter at the start.
		Invoke("CreateStartButton", 1f);
	}

	void CreateStartButton()
	{
		GameObject startButtonObj = Instantiate(startButtonPrefab) as GameObject;
		startButtonObj.name = "Start Button";
		startButtonObj.transform.parent = transform;
		startButtonObj.transform.position = new Vector3(0f, Constants.I.StartButtonLoweredY, 0f);

		startButton = startButtonObj.GetComponent(typeof(DebugStartButton)) as DebugStartButton;
		startButton.Raise();
	}

	public void OnGameEnd()
	{
		startButton.Raise();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			BeginGame();
		}
	}

	public void BeginGame()
	{
		if (game == null)
		{
			Debug.Log("[GAME MANAGER] Loading and Beginning Game.");

			game = Instantiate(gamePrefab) as Game;
			game.name = "Chessumo Game";
			game.transform.parent = transform;

			game.Load();
		}
		else
		{
			Debug.Log("[GAME MANAGER] Unloading Game");
			game.Unload();

			game = null;

			// Reload the game, now that we've unloaded everything correctly.
			BeginGame();
		}
	}

	public Vector2 CoordinateToPosition(IntVector2 coordinate)
	{
		//float xPos = coordinate.x - Mathf.Floor(Constants.I.GRID_SIZE.x / 2f) + Constants.I.GRID_OFFSET_X;
		//float yPos = coordinate.y - Mathf.Floor(Constants.I.GRID_SIZE.y / 2f) + Constants.I.GRID_OFFSET_Y;

		float xPos = (coordinate.x - (Constants.I.GridSize.x / 2f)) + 0.5f + Constants.I.GridOffsetX;
		float yPos = (coordinate.y - (Constants.I.GridSize.y / 2f)) + 0.5f + Constants.I.GridOffsetY;
		return new Vector2(xPos, yPos);
	}

	public void GrowMe(GameObject obj, float delay = 0f, Ease ease = Ease.OutBack)
	{
		StartCoroutine(GrowToScale(obj, delay, ease));
	}

	IEnumerator GrowToScale(GameObject obj, float delay, Ease ease)
	{
		// Neat little effect for now to compensate for the fact that shit would just appear out of nowhere otherwise. this will die someday.

		if (!obj)
		{
			yield break;
		}

		Vector3 startScale = new Vector3(0f, 0f, 1f);
		Vector3 desiredScale = obj.transform.localScale;

		// Scale to 0 first.
		obj.transform.localScale = startScale;

		// Then wait the delay.
		yield return new WaitForSeconds(delay);

		// Then rescale via tween.
		if (obj)
		{
			obj.transform.DOScale(desiredScale, 1f)
				.SetEase(ease);
		}

		yield return null;
	}

	public void GrowMeFromSlit(GameObject obj, float delay = 0f, Ease ease = Ease.OutBack)
	{
		StartCoroutine(GrowFromSlit(obj, delay, ease));
	}

	IEnumerator GrowFromSlit(GameObject obj, float delay, Ease ease)
	{
		if (!obj)
		{
			yield break;
		}

		Vector3 startScale = new Vector3(obj.transform.localScale.x, 0f, 1f);
		float desiredScaleY = obj.transform.localScale.y;

		// Scale to 0 first.
		obj.transform.localScale = startScale;

		// Then wait the delay.
		yield return new WaitForSeconds(delay);

		// Then rescale via tween.
		if (obj)
		{
			obj.transform.DOScaleY(desiredScaleY, 1f)
				.SetEase(ease);
		}

		yield return null;
	}

	public void ShrinkMeToSlit(GameObject obj, float delay = 0f, Ease ease = Ease.OutBack)
	{
		StartCoroutine(ShrinkToSlit(obj, delay, ease));
	}

	IEnumerator ShrinkToSlit(GameObject obj, float delay, Ease ease)
	{
		// Then wait the delay.
		yield return new WaitForSeconds(delay);

		// Then rescale via tween.
		if (obj)
		{
			obj.transform.DOScaleY(0f, 1f)
				.SetEase(ease);
		}

		yield return null;
	}

	public void IntroduceFromSide(GameObject obj, float delay, bool isRight, Ease ease = Ease.OutBounce)
	{
		StartCoroutine(IntroduceFromSideRoutine(obj, delay, isRight, ease));
	}

	IEnumerator IntroduceFromSideRoutine(GameObject obj, float delay, bool isRight, Ease ease)
	{
		int sign = isRight ? 1 : -1;
		Vector3 startPos = new Vector3(5f * sign, obj.transform.position.y, obj.transform.position.z);
		Vector3 endPos = obj.transform.position;

		// send off to the side first.
		obj.transform.position = startPos;

		// Then wait the delay.
		yield return new WaitForSeconds(delay);

		// Then rescale via tween.
		if (obj)
		{
			obj.transform.DOMove(endPos, 1f)
				.SetEase(ease);
		}

		yield return null;
	}
}
















