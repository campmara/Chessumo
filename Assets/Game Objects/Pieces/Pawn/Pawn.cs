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
		List<IntVector2> diagonals = parentGame.GetDiagonalPieceCoordinates(currentCoordinates);

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

	public override void DetermineMoveset()
	{
		moveset = new InitialMove();

		moveset.up = GetUp(moveset);
		moveset.down = GetDown(moveset);

		// Check for diagonal pieces.
		moveset.up_left = GetUpLeft(moveset);
		moveset.up_right = GetUpRight(moveset);
		moveset.down_left = GetDownLeft(moveset);
		moveset.down_right = GetDownRight(moveset);
	}

	protected override MoveUpLeft GetUpLeft(InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords) && parentGame.CoordsOccupied(newCoords))
		{
			MoveUpLeft ret = new MoveUpLeft();
			ret.coordinates = newCoords;
			ret.moveOffset = offset;
			ret.reverse = init;
			return ret;
		}
		else
		{
			return null;
		}
	}

	protected override MoveUpRight GetUpRight(InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords) && parentGame.CoordsOccupied(newCoords))
		{
			MoveUpRight ret = new MoveUpRight();
			ret.coordinates = newCoords;
			ret.moveOffset = offset;
			ret.reverse = init;
			return ret;
		}
		else
		{
			return null;
		}
	}

	protected override MoveDownLeft GetDownLeft(InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords) && parentGame.CoordsOccupied(newCoords))
		{
			MoveDownLeft ret = new MoveDownLeft();
			ret.coordinates = newCoords;
			ret.moveOffset = offset;
			ret.reverse = init;
			return ret;
		}
		else
		{
			return null;
		}
	}

	protected override MoveDownRight GetDownRight(InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords) && parentGame.CoordsOccupied(newCoords))
		{
			MoveDownRight ret = new MoveDownRight();
			ret.coordinates = newCoords;
			ret.moveOffset = offset;
			ret.reverse = init;
			return ret;
		}
		else
		{
			return null;
		}
	}
}
