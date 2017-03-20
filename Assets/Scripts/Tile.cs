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
	[SerializeField] private Color alternateColor = Color.grey;

	private SpriteRenderer spriteRenderer;

	private Mode parentMode;
	private IntVector2 currentCoordinates;

	void Awake()
	{
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
		SetColorDefault();
	}

	public void SetColorDefault()
	{
		spriteRenderer.color = color;
	}

	public void SetColorAlternate()
	{
		spriteRenderer.color = alternateColor;
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
		return spriteRenderer.sprite == upSprite;
	}

	// Public because this gets called by the mode.
	public void ShowMove()
	{
		if (!IsShowingMove())
		{
			spriteRenderer.sprite = upSprite;
			SetColorAlternate();
		}
	}

	public void HideMove()
	{
		if (IsShowingMove())
		{
			spriteRenderer.sprite = downSprite;
			SetColorDefault();
		}
	}
}
