using UnityEngine;
using System.Collections;

public class Rook : Piece 
{
	protected override void Awake()
	{
		base.Awake();

		// This piece can move pretty much across the board;
		moveMagnitude = 10;

		moveOffsets = new IntVector2[4];

		moveOffsets[0] = new IntVector2(0, 1);
		moveOffsets[1] = new IntVector2(0, -1);
		moveOffsets[2] = new IntVector2(1, 0);
		moveOffsets[3] = new IntVector2(-1, 0);
	}

	public override void DetermineMoveset()
	{
		moveset = new InitialMove();

		moveset.up = GetUp(moveset);
		moveset.down = GetDown(moveset);
		moveset.left = GetLeft(moveset);
		moveset.right = GetRight(moveset);
	}
}













