using UnityEngine;
using System.Collections;

public class Knight : Piece 
{
	protected override void Awake()
	{
		base.Awake();

		moveMagnitude = 1;

		moveOffsets = new IntVector2[8];

		moveOffsets[0] = new IntVector2(-1, 2);
		moveOffsets[1] = new IntVector2(1, 2);
		moveOffsets[2] = new IntVector2(2, 1);
		moveOffsets[3] = new IntVector2(2, -1);
		moveOffsets[4] = new IntVector2(1, -2);
		moveOffsets[5] = new IntVector2(-1, -2);
		moveOffsets[6] = new IntVector2(-2, 1);
		moveOffsets[7] = new IntVector2(-2, -1);
	}
}