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
	public GameObject tilePrefab;
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

	GameObject selectionObj;

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			BeginGame();
		}
	}

	void BeginGame()
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
		selectionObj.SetActive(false);
	}
}
















