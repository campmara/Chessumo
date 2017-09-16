using System.Collections;
using System.Collections.Generic;

public class Move
{
	public IntVector2 coordinates; // The coordinates of wherever this move is.
	public IntVector2 moveOffset; // The offset needed to be applied to get to the current coords.

	public Move reverse = null; // The last move before this one, set on the fly for undoing moves and going back to the prev.
	public bool isPossibleEnd = false; // This is the last move in a possible piece movement. If we lift finger, we can move here.

	public Move()
	{

	}
}

public class InitialMove : Move
{
	public MoveUp up = null;
	public MoveDown down = null;
	public MoveLeft left = null;
	public MoveRight right = null;
	public MoveUpLeft up_left = null;
	public MoveUpRight up_right = null;
	public MoveDownLeft down_left = null;
	public MoveDownRight down_right = null;

	// For easier, non-recursive access to all possible moves when we need them.
	public List<Move> moveList;

	public InitialMove() : base()
	{
		moveList = new List<Move>();
	}

	// Searches the list of moves to find the one at the specified coordinates. Nifty!
	public Move DetermineMove(IntVector2 coordinates)
	{
		for (int i = 0; i < moveList.Count; i++)
		{
			if (moveList[i].coordinates == coordinates)
			{
				return moveList[i];
			}
		}

		return null;
	}

	public Move DetermineKnightMove(IntVector2 coordinates, IntVector2 offset)
	{
		for (int i = 0; i < moveList.Count; i++)
		{
			if (moveList[i].coordinates == coordinates && offset == moveList[i].moveOffset)
			{
				return moveList[i];
			}
		}

		return null;
	}
}

public class MoveUp : Move
{
	public MoveUp up = null;

	public MoveLeft left = null;
	public MoveRight right = null;
}

public class MoveDown : Move
{
	public MoveDown down = null;

	public MoveLeft left = null;
	public MoveRight right = null;
}

public class MoveLeft : Move
{
	public MoveLeft left = null;

	public MoveUp up = null;
	public MoveDown down = null;
}

public class MoveRight : Move
{
	public MoveRight right = null;

	public MoveUp up = null;
	public MoveDown down = null;
}

public class MoveUpLeft : Move
{
	public MoveUpLeft up_left;
}

public class MoveUpRight : Move
{
	public MoveUpRight up_right;
}

public class MoveDownLeft : Move
{
	public MoveDownLeft down_left;
}

public class MoveDownRight : Move
{
	public MoveDownRight down_right;
}