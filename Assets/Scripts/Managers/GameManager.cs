using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// CONSTANTS
	/////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////
	// PUBLICS
	/////////////////////////////////////////////////////////////////////

	public static GameManager Instance = null;

	[Header("General Prefabs")]
	public GameObject startButtonPrefab;
	public GameObject tilePrefab;
	public GameObject scorePrefab;
	public GameObject nextPieceViewerPrefab;
	[Header("Piece Prefabs")]
	public GameObject kingPrefab;
	public GameObject queenPrefab;
	public GameObject rookPrefab;
	public GameObject bishopPrefab;
	public GameObject knightPrefab;
	public GameObject pawnPrefab;
	[Header("Effect Prefabs")]
	public GameObject selectionPrefab;
	public GameObject possibleMovePrefab;

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////

	GameObject selectionObj;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		CreateStartButton();
	}

	void CreateStartButton()
	{
		GameObject startButton = Instantiate(startButtonPrefab) as GameObject;
		startButton.name = "Start Button";
		startButton.transform.parent = transform;
		startButton.transform.position = new Vector3(0f, -3.5f, 0f);
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
		ModeManager.Instance.Load();
	}

	public Vector2 CoordinateToPosition(IntVector2 coordinate)
	{
		float xPos = coordinate.x - Mathf.Floor(ModeManager.Instance.CurrentMode.GridSize.x / 2f);
		float yPos = coordinate.y - Mathf.Floor(ModeManager.Instance.CurrentMode.GridSize.y / 2f);
		return new Vector2(xPos, yPos);
	}

	public void SelectObject(Transform obj)
	{
		if (!selectionObj)
		{
			selectionObj = Instantiate(selectionPrefab) as GameObject;
			selectionObj.name = "Selection";
			selectionObj.transform.parent = transform;
			selectionObj.SetActive(false);
		}

		selectionObj.transform.position = obj.position;
		//selectionObj.transform.localScale = new Vector2(obj.localScale.x + 0.1f, obj.localScale.y + 0.1f);
		selectionObj.SetActive(true);
	}

	public void Deselect()
	{
		if (selectionObj)
			selectionObj.SetActive(false);
	}

	public void GrowMe(GameObject obj)
	{
		StartCoroutine(GrowToScale(obj));
	}

	IEnumerator GrowToScale(GameObject obj)
	{
		// Neat little effect for now to compensate for the fact that shit would just appear out of nowhere otherwise. this will die someday.

		Vector3 startScale = new Vector3(0f, 0f, 1f);
		Vector3 desiredScale = obj.transform.localScale;

		obj.transform.localScale = startScale;

		float timer = 0f;
		float totalTime = 0.3f;

		while (timer < totalTime)
		{
			if (obj)
			{
				timer += Time.deltaTime;
				obj.transform.localScale = Vector3.Lerp(startScale, desiredScale, timer / totalTime);
			}

			yield return null;
		}
	}
}
















