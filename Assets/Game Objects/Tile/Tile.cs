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

	[SerializeField] private Sprite downSprite;
	[SerializeField] private Sprite upSprite;

	[SerializeField] private Color color = Color.white;
	[SerializeField] private Color possibleColor = Color.grey;
	[SerializeField] private Color fingerColor = Color.grey;

	private SpriteRenderer spriteRenderer;

	private Game parentGame;
	private IntVector2 currentCoordinates;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		SetColorDefault();
	}

	public void SetColorDefault()
	{
		spriteRenderer.color = color;
		spriteRenderer.sortingOrder = 0;
	}

	public void SetColorPossible()
	{
		spriteRenderer.color = possibleColor;
		spriteRenderer.sortingOrder = 0;
	}

	public void SetColorFinger()
	{
		spriteRenderer.color = fingerColor;
		spriteRenderer.sortingOrder = 1;
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMe(this.gameObject, Random.Range(0f, 0.5f));
	}

	public IntVector2 GetCoordinates()
	{
		return currentCoordinates;
	}

	public void SetInfo(int x, int y, Game game)
	{
		currentCoordinates = new IntVector2(x, y);
		parentGame = game;
	}

	public bool IsShowingPossibleMove()
	{
		return spriteRenderer.color == possibleColor;
	}

	// Public because this gets called by the mode.
	public void ShowPossibleMove()
	{
		if (!IsShowingPossibleMove())
		{
			SetColorPossible();
		}
	}

	public void HidePossibleMove()
	{
		if (IsShowingPossibleMove())
		{
			spriteRenderer.sprite = downSprite;
			SetColorDefault();
		}
	}

	public bool IsShowingFingerMove()
	{
		return spriteRenderer.sprite == upSprite;
	}

	public void ShowFingerMove()
	{
		if (IsShowingPossibleMove() && !IsShowingFingerMove())
		{
			spriteRenderer.sprite = upSprite;
			SetColorFinger();
		}
	}

	public void HideFingerMove()
	{
		if (IsShowingFingerMove())
		{
			spriteRenderer.sprite = downSprite;
			SetColorPossible();
		}
	}

	public void HideAllEffects()
	{
		spriteRenderer.sprite = downSprite;
		SetColorDefault();
	}
}
