using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
	//////////////////////////////////////////////////////////
	// CONSTANTS
	//////////////////////////////////////////////////////////



	//////////////////////////////////////////////////////////
	// PUBLICS
	//////////////////////////////////////////////////////////



	//////////////////////////////////////////////////////////
	// PRIVATES
	//////////////////////////////////////////////////////////

	[SerializeField] Color color = Color.white;
	[SerializeField] Color highlightColor = Color.white;
	[SerializeField] Color selectedColor = Color.white;

	SpriteRenderer sprite;

	Mode parentMode;
	IntVector2 currentCoordinates;

	GameObject possibleMoveObj;

	void Awake()
	{
		sprite = GetComponentInChildren<SpriteRenderer>();
		Default();
	}

	void Update()
	{
		if (Input.GetMouseButtonUp(0) && !IsShowingMove())
		{
			Default();
		}
	}

	public IntVector2 GetCoordinates()
	{
		return currentCoordinates;
	}

	public void SetInfo(int x, int y, Mode mode)
	{
		currentCoordinates = new IntVector2(x, y);
		parentMode = mode;

		possibleMoveObj = Instantiate(GameManager.Instance.possibleMovePrefab) as GameObject;
		possibleMoveObj.transform.parent = transform;
		possibleMoveObj.transform.localPosition = Vector2.zero;
		possibleMoveObj.SetActive(false);
	}

	void OnMouseEnter()
	{
		if (Input.GetMouseButton(0))
		{
			Select();
		}
		else
		{
			Highlight();
		}
	}

	void OnMouseOver()
	{
		if (!Input.GetMouseButton(0))
			Highlight();
	}

	void OnMouseDown()
	{
		Select();
	}

	void OnMouseUp()
	{
		Default();
	}

	void OnMouseExit()
	{
		if (!Input.GetMouseButton(0))
			Default();
	}

	//////////////////////////////////////////////////////////
	// SELECTION VARIANTS
	//////////////////////////////////////////////////////////

	void Default()
	{
		sprite.color = color;
	}

	void Highlight()
	{
		sprite.color = highlightColor;
	}

	void Select()
	{
		sprite.color = selectedColor;
	}

	public bool IsShowingMove()
	{
		return possibleMoveObj.activeSelf;
	}

	// Public because this gets called by the mode.
	public void ShowMove()
	{
		if (!IsShowingMove())
		{
			possibleMoveObj.SetActive(true);
		}
	}

	public void HideMove()
	{
		if (IsShowingMove())
		{
			possibleMoveObj.SetActive(false);
		}
	}
}
