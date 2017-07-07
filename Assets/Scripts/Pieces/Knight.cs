﻿using UnityEngine;
using System.Collections;

public class Knight : Piece 
{
	IntVector2[] secondaryMoveOffsets;

	IntVector2 initialDirection;

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

		initialDirection = IntVector2.NULL;
	}

	// This is necessarily long.
	public override void MoveTo(IntVector2 coordinates, bool pushed)
	{
		if (!pushed && !HasDirection())
		{
			Debug.LogError("[KNIGHT]: initial direction not set on move...");
			return;
		}

		// Determine where it is we're actually moving.
		IntVector2 diff = coordinates - GetCoordinates();
		
		//
		// PUSHED
		//

		if (pushed)
		{
			if (moveDisabled)
			{
				SetMoveDisabled(false);
			}

			// Get Pushed.

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
			SetMoveDisabled(true);
		}
			
		//
		// MOVED
		//

		if (initialDirection.x != 0)
		{
			// Move horizontally first, then vertically.
			StartCoroutine(MoveXThenY(diff.x, diff.y));
			ResetKnight();
		}
		else if (initialDirection.y != 0)
		{
			// Move vertically first, then horizontally.
			StartCoroutine(MoveYThenX(diff.x, diff.y));
			ResetKnight();
		}
	}

	public void SetInitialDirection(IntVector2 dir)
	{
		initialDirection = dir;
	}

	public bool HasDirection()
	{
		return initialDirection != IntVector2.NULL;
	}

	public void ResetKnight()
	{
		initialDirection = IntVector2.NULL;
	}
}