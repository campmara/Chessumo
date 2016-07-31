using UnityEngine;
using System.Collections;

public class Bishop : Piece 
{
	protected override void Awake()
	{
		base.Awake();

		moveMagnitude = 10;

		moveOffsets = new IntVector2[4];
		moveOffsets[0] = new IntVector2(-1, 1);
		moveOffsets[1] = new IntVector2(1, 1);
		moveOffsets[2] = new IntVector2(-1, -1);
		moveOffsets[3] = new IntVector2(1, -1);
	}
}