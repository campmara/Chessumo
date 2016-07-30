using UnityEngine;
using System.Collections;

public class Pawn : Piece 
{
	void Awake()
	{
		moveMagnitude = 1;

		moveOffsets = new IntVector2[1];
		moveOffsets[0] = new IntVector2(0, 1);
	}
}
