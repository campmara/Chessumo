using UnityEngine;
using System.Collections;

public class King : Piece 
{
	void Awake()
	{
		moveMagnitude = 1;
		
		moveOffsets = new IntVector2[4];

		moveOffsets[0] = new IntVector2(0, 1);
		moveOffsets[1] = new IntVector2(0, -1);
		moveOffsets[2] = new IntVector2(1, 0);
		moveOffsets[3] = new IntVector2(-1, 0);
	}
}
