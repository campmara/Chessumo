using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pawn : Piece 
{
	protected override void Awake()
	{
		base.Awake();

		// we don't need to set the move magnitude if it's 1.
		//moveMagnitude = 1;

		moveOffsets = new IntVector2[2];
		moveOffsets[0] = new IntVector2(0, 1);
		moveOffsets[1] = new IntVector2(0, -1);
	}

	// This has to be overriden because the pawn's possible moves are sometimes diagonal if there is a piece to push there.
	public override IntVector2[] GetPossibleMoves()
	{
		// Check for diagonal pieces. Put them in a new array of moveOffsets that the rest of this function uses.
		List<IntVector2> diagonals = parentMode.GetDiagonalPieceCoordinates(currentCoordinates);

		IntVector2[] returnArray = new IntVector2[moveOffsets.Length + diagonals.Count];

		// Add move offsets to array.
		for (int i = 0; i < moveOffsets.Length; i++)
		{
			returnArray[i] = currentCoordinates + moveOffsets[i];
		}

		// Add diagonals to array.
		int currentIndex = moveOffsets.Length;
		for (int i = 0; i < diagonals.Count; i++)
		{
			returnArray[currentIndex] = diagonals[i];
			currentIndex++;
		}

		return returnArray;
	}
}
