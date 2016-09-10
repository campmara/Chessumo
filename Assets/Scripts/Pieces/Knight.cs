using UnityEngine;
using System.Collections;

public class Knight : Piece 
{
	IntVector2[] secondaryMoveOffsets;

	IntVector2 destination;
	IntVector2 direction;

	protected override void Awake()
	{
		base.Awake();

		//moveMagnitude = 1;

		moveOffsets = new IntVector2[8];

		moveOffsets[0] = new IntVector2(-1, 2);
		moveOffsets[1] = new IntVector2(1, 2);
		moveOffsets[2] = new IntVector2(2, 1);
		moveOffsets[3] = new IntVector2(2, -1);
		moveOffsets[4] = new IntVector2(1, -2);
		moveOffsets[5] = new IntVector2(-1, -2);
		moveOffsets[6] = new IntVector2(-2, 1);
		moveOffsets[7] = new IntVector2(-2, -1);

		// Defined later.
		secondaryMoveOffsets = new IntVector2[2];

		destination = IntVector2.NULL;
		direction = IntVector2.NULL;
	}

	public override IntVector2[] GetPossibleMoves()
	{
		IntVector2[] returnArray;

		if (destination == IntVector2.NULL)
		{
			returnArray = new IntVector2[moveOffsets.Length];

			for (int i = 0; i < moveOffsets.Length; i++)
			{
				returnArray[i] = currentCoordinates + moveOffsets[i];
			}
		}
		else
		{
			returnArray = new IntVector2[secondaryMoveOffsets.Length];

			for (int i = 0; i < secondaryMoveOffsets.Length; i++)
			{
				returnArray[i] = currentCoordinates + secondaryMoveOffsets[i];
			}
		}

		return returnArray;
	}

	// This is necessarily long.
	public override void MoveTo(IntVector2 coordinates, bool pushed)
	{
		//
		// PUSHED
		//

		if (pushed)
		{
			if (moveDisabled)
			{
				SetMoveDisabled(false);
			}

			// Determine how many times we must repeat the movement to get to the desired point.
			IntVector2 diff = coordinates - GetCoordinates();

			if (diff.x != 0 && diff.y == 0)
			{
				// Move horizontally.
				StartCoroutine(MoveX(diff.x));
			}
			else if (diff.y != 0 && diff.x == 0)
			{
				// Move vertically.
				StartCoroutine(MoveY(diff.y));
			}
			else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) == Mathf.Abs(diff.y))
			{
				// Move diagonally.
				StartCoroutine(MoveDiagonally(diff.x, diff.y));
			}

			return;
		}
		else
		{
			if (HasDestination())
			{
				SetMoveDisabled(true);
			}
		}
			
		//
		// MOVED
		//

		if (destination == IntVector2.NULL)
		{
			destination = coordinates;

			IntVector2 diff = destination - currentCoordinates;

			if (diff.x > 0 && diff.y > 0)
			{
				secondaryMoveOffsets[0] = new IntVector2(1, 0);
				secondaryMoveOffsets[1] = new IntVector2(0, 1);
			}
			else if (diff.x > 0 && diff.y < 0)
			{
				secondaryMoveOffsets[0] = new IntVector2(1, 0);
				secondaryMoveOffsets[1] = new IntVector2(0, -1);
			}
			else if (diff.x < 0 && diff.y < 0)
			{
				secondaryMoveOffsets[0] = new IntVector2(-1, 0);
				secondaryMoveOffsets[1] = new IntVector2(0, -1);
			}
			else if (diff.x < 0 && diff.y > 0)
			{
				secondaryMoveOffsets[0] = new IntVector2(-1, 0);
				secondaryMoveOffsets[1] = new IntVector2(0, 1);
			}
		}
		else
		{
			direction = coordinates - currentCoordinates;

			IntVector2 diff = destination - currentCoordinates;

			if (direction.x != 0)
			{
				// Move horizontally first, then vertically.
				StartCoroutine(MoveXThenY(diff.x, diff.y));
			}
			else if (direction.y != 0)
			{
				// Move vertically first, then horizontally.
				StartCoroutine(MoveYThenX(diff.x, diff.y));
			}
		}
	}

	public bool HasDestination()
	{
		return destination != IntVector2.NULL;
	}

	public bool HasDirection()
	{
		return direction != IntVector2.NULL;
	}

	public void ResetKnight()
	{
		destination = IntVector2.NULL;
		direction = IntVector2.NULL;
	}
}