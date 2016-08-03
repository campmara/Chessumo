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

	[SerializeField] GameObject possibleMoveObj;

	[SerializeField] Color color = Color.white;
	[SerializeField] Color alternateColor = Color.grey;

	SpriteRenderer sprite;

	Mode parentMode;
	IntVector2 currentCoordinates;

	void Awake()
	{
		sprite = GetComponentInChildren<SpriteRenderer>();
	}

	public void SetColorDefault()
	{
		sprite.color = color;
	}

	public void SetColorAlternate()
	{
		sprite.color = alternateColor;
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMe(this.gameObject);
	}

	public IntVector2 GetCoordinates()
	{
		return currentCoordinates;
	}

	public void SetInfo(int x, int y, Mode mode)
	{
		currentCoordinates = new IntVector2(x, y);
		parentMode = mode;
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
