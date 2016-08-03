using UnityEngine;
using System.Collections;

public class NextPieceViewer : MonoBehaviour 
{
	[SerializeField] SpriteRenderer pieceSprite;

	[SerializeField] Sprite kingSprite;
	[SerializeField] Sprite queenSprite;
	[SerializeField] Sprite rookSprite;
	[SerializeField] Sprite bishopSprite;
	[SerializeField] Sprite knightSprite;
	[SerializeField] Sprite pawnSprite;
	
	void Awake()
	{
		if (!pieceSprite)
		{
			Debug.LogError("NEXTPIECEVIEWER : You forgot to assign the piece sprite.");
		}
	}

	void OnEnable()
	{
		GameManager.Instance.GrowMe(this.gameObject);
	}

	public void ShowKing()
	{
		pieceSprite.sprite = kingSprite;
	}

	public void ShowQueen()
	{
		pieceSprite.sprite = queenSprite;
	}

	public void ShowRook()
	{
		pieceSprite.sprite = rookSprite;
	}

	public void ShowBishop()
	{
		pieceSprite.sprite = bishopSprite;
	}

	public void ShowKnight()
	{
		pieceSprite.sprite = knightSprite;
	}

	public void ShowPawn()
	{
		pieceSprite.sprite = pawnSprite;
	}
}
