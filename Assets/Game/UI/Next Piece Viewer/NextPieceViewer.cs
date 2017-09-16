using UnityEngine;
using System.Collections;
using DG.Tweening;

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
		GameManager.Instance.GrowMeFromSlit(this.gameObject, 2f, Ease.OutBounce);
	}

	public void PositionAlongTop(float xPos)
	{
		transform.DOMoveX(xPos, 0.75f).SetEase(Ease.InOutQuint);
		//transform.position = new Vector3(xPos, transform.position.y, 0f);
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
