using UnityEngine;
using System.Collections;
using DG.Tweening;

public class NextPieceViewer : MonoBehaviour 
{
	[SerializeField] SpriteRenderer sprite;

	[SerializeField] Color kingColor;
	[SerializeField] Color queenColor;
	[SerializeField] Color rookColor;
	[SerializeField] Color bishopColor;
	[SerializeField] Color knightColor;
	[SerializeField] Color pawnColor;

	public void PositionAlongTop(float xPos)
	{
		transform.DOMoveX(xPos, 0.75f).SetEase(Ease.InOutQuint);
	}

	public void ShowKing()
	{
		sprite.DOColor(kingColor, 0.75f);
	}

	public void ShowQueen()
	{
		sprite.DOColor(queenColor, 0.75f);
	}

	public void ShowRook()
	{
		sprite.DOColor(rookColor, 0.75f);
	}

	public void ShowBishop()
	{
		sprite.DOColor(bishopColor, 0.75f);
	}

	public void ShowKnight()
	{
		sprite.DOColor(knightColor, 0.75f);
	}

	public void ShowPawn()
	{
		sprite.DOColor(pawnColor, 0.75f);
	}
}
