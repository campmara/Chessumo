using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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

public abstract class Piece : MonoBehaviour 
{
	public Color FullColor { get { return fullColor; } }
	public Color SubduedColor { get { return subduedColor; } }
	[Header("Piece Colors"), SerializeField] private Color fullColor = Color.white;
	[SerializeField] private Color subduedColor = Color.white;

	[Header("Other Variables"), SerializeField] Color tint = Color.white;
	[SerializeField] Color disabledTint = Color.black;
	[ReadOnly] public bool potentialPush = false;

	public InitialMove Moveset { get { return moveset; } }
	protected InitialMove moveset;

	// Array of coordinate offsets that define the moves a piece can make.
	protected IntVector2[] moveOffsets;

	protected int moveMagnitude = 1;
	[SerializeField] protected IntVector2 currentCoordinates;
	protected Game parentGame;

	protected bool moveDisabled = false;

	SpriteRenderer sprite;

	protected virtual void Awake()
	{
		sprite = GetComponentInChildren<SpriteRenderer>();
		sprite.color = tint;
	}

	public IntVector2 GetCoordinates()
	{
		return currentCoordinates;
	}

	public void SetCoordinates(IntVector2 coordinates)
	{
		parentGame.UpdatePieceCoordinates(this, currentCoordinates, coordinates);
		currentCoordinates = coordinates;
	}

	public void SetInfo(int x, int y, Game game)
	{
		currentCoordinates = new IntVector2(x, y);
		parentGame = game;
	}

	public void HandleFallingSortingOrder()
	{
		sprite.sortingLayerName = "Falling Pieces";
	}

	public void SetPushPotential(bool hasPotential)
	{
		potentialPush = hasPotential;
	}

	// For when it's underneath a tile that's highlighted.
	public void PickPieceUp()
	{
		sprite.transform.localPosition += Vector3.up * 0.1f;
	}

	public void SetPieceDown()
	{
		sprite.transform.localPosition = Vector3.zero;
	}

	void OnEnable()
	{
		// Spawn in 1 second after the tiles do.
		GameManager.Instance.GrowMe(this.gameObject);
	}
	/*
	public virtual IntVector2[] GetPossibleMoves()
	{
		IntVector2[] returnArray = new IntVector2[moveOffsets.Length * moveMagnitude];

		for (int i = 0; i < returnArray.Length; i++)
		{
			int offsetIndex = Mathf.FloorToInt(i / moveMagnitude);
			int coefficient = moveMagnitude > 1 ? (i + 1) % moveMagnitude : 1;
			coefficient = coefficient == 0 ? moveMagnitude : coefficient;
			IntVector2 moveOffset = new IntVector2(moveOffsets[offsetIndex].x * coefficient, moveOffsets[offsetIndex].y * coefficient);
			//Debug.Log("Move Offset: [" + moveOffset.x + ", " + moveOffset.y + "]");

			returnArray[i] = currentCoordinates + moveOffset;
		}

		return returnArray;
	}
	*/

	public virtual List<IntVector2> GetPossibleMoves()
	{
		List<IntVector2> list = new List<IntVector2>();

		for (int i = 0; i < moveset.moveList.Count; i++)
		{
			if (moveset.moveList[i].isPossibleEnd)
			{
				list.Add(moveset.moveList[i].coordinates);
			}
		}

		return list;
	}

	public virtual void DetermineMoveset()
	{

	}

	protected MoveUp GetUp(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(0, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveUp[] moves = new MoveUp[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveUp up = new MoveUp();
				up.coordinates = newCoords;
				up.moveOffset = offset;
				up.isPossibleEnd = true;
				moves[i] = up;
				if (i == 0)
				{
					up.reverse = init;
				}
				else
				{
					moves[i - 1].up = up;
					up.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(up);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected MoveDown GetDown(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(0, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveDown[] moves = new MoveDown[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveDown down = new MoveDown();
				down.coordinates = newCoords;
				down.moveOffset = offset;
				down.isPossibleEnd = true;
				moves[i] = down;
				if (i == 0)
				{
					down.reverse = init;
				}
				else
				{
					moves[i - 1].down = down;
					down.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(down);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected MoveLeft GetLeft(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, 0);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveLeft[] moves = new MoveLeft[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveLeft left = new MoveLeft();
				left.coordinates = newCoords;
				left.moveOffset = offset;
				left.isPossibleEnd = true;
				moves[i] = left;
				if (i == 0)
				{
					left.reverse = init;
				}
				else
				{
					moves[i - 1].left = left;
					left.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(left);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected MoveRight GetRight(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, 0);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveRight[] moves = new MoveRight[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveRight right = new MoveRight();
				right.coordinates = newCoords;
				right.moveOffset = offset;
				right.isPossibleEnd = true;
				moves[i] = right;
				if (i == 0)
				{
					right.reverse = init;
				}
				else
				{
					moves[i - 1].right = right;
					right.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(right);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected virtual MoveUpRight GetUpRight(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveUpRight[] moves = new MoveUpRight[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveUpRight up_right = new MoveUpRight();
				up_right.coordinates = newCoords;
				up_right.moveOffset = offset;
				up_right.isPossibleEnd = true;
				moves[i] = up_right;
				if (i == 0)
				{
					up_right.reverse = init;
				}
				else
				{
					moves[i - 1].up_right = up_right;
					up_right.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(up_right);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected virtual MoveDownRight GetDownRight(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveDownRight[] moves = new MoveDownRight[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveDownRight down_right = new MoveDownRight();
				down_right.coordinates = newCoords;
				down_right.moveOffset = offset;
				down_right.isPossibleEnd = true;
				moves[i] = down_right;
				if (i == 0)
				{
					down_right.reverse = init;
				}
				else
				{
					moves[i - 1].down_right = down_right;
					down_right.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(down_right);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected virtual MoveDownLeft GetDownLeft(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveDownLeft[] moves = new MoveDownLeft[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveDownLeft down_left = new MoveDownLeft();
				down_left.coordinates = newCoords;
				down_left.moveOffset = offset;
				down_left.isPossibleEnd = true;
				moves[i] = down_left;
				if (i == 0)
				{
					down_left.reverse = init;
				}
				else
				{
					moves[i - 1].down_left = down_left;
					down_left.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(down_left);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	protected virtual MoveUpLeft GetUpLeft(ref InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		MoveUpLeft[] moves = new MoveUpLeft[moveMagnitude];

		for (int i = 0; i < moveMagnitude; i++)
		{
			if (parentGame.IsWithinBounds(newCoords))
			{
				MoveUpLeft up_left = new MoveUpLeft();
				up_left.coordinates = newCoords;
				up_left.moveOffset = offset;
				up_left.isPossibleEnd = true;
				moves[i] = up_left;
				if (i == 0)
				{
					up_left.reverse = init;
				}
				else
				{
					moves[i - 1].up_left = up_left;
					up_left.reverse = moves[i - 1];
				}

				// Add this move to the list of this moveset's moves.
				init.moveList.Add(up_left);

				// Increment coords to check the next space up.
				newCoords = newCoords + offset;
			}
			else
			{
				break;
			}
		}

		return moves[0];
	}

	public virtual void MoveTo(IntVector2 coordinates, bool pushed, int distFromPushingPiece, int numPushedPieces)
	{
		// This is the core disable / pushing logic.
		if (pushed)
		{
			if (moveDisabled)
			{
				SetMoveDisabled(false);
			}
		}

		// Determine how many times we must repeat the movement to get to the desired point.
		IntVector2 diff = coordinates - GetCoordinates();

		if (diff.x != 0 && diff.y == 0)
		{
			// Move horizontally.
			StartCoroutine(MoveX(diff.x, distFromPushingPiece, numPushedPieces, pushed));
		}
		else if (diff.y != 0 && diff.x == 0)
		{
			// Move vertically.
			StartCoroutine(MoveY(diff.y, distFromPushingPiece, numPushedPieces, pushed));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
		{
			// Move horizontally first, then vertically.
			StartCoroutine(MoveXThenY(diff.x, diff.y, pushed));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) < Mathf.Abs(diff.y))
		{
			// Move vertically first, then horizontally.
			StartCoroutine(MoveYThenX(diff.x, diff.y, pushed));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) == Mathf.Abs(diff.y))
		{
			// Move diagonally.
			StartCoroutine(MoveDiagonally(diff.x, diff.y, distFromPushingPiece, numPushedPieces, pushed));
		}
	}

	public bool GetMoveDisabled()
	{
		return moveDisabled;
	}

	public void SetMoveDisabled(bool disabled)
	{
		moveDisabled = disabled;
		if (disabled)
		{
			sprite.DOColor(disabledTint, 0.75f);
			//sprite.color = disabledTint;
		}
		else
		{
			sprite.DOColor(tint, 0.75f);
			//sprite.color = tint;
		}
	}

	protected IEnumerator MoveXThenY(int xDistance, int yDistance, bool pushed)
	{
		///////////////////////////////
		// X MOVEMENT
		///////////////////////////////

		int sign = Mathf.FloorToInt(Mathf.Sign(xDistance));
		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(xDistance, 0);
		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(sign, 0), xDistance);
		SetCoordinates(nextCoordinates);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(xDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		///////////////////////////////
		// Y MOVEMENT
		///////////////////////////////

		sign = Mathf.FloorToInt(Mathf.Sign(yDistance));
		nextCoordinates = currentCoordinates + new IntVector2(0, yDistance);
		convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(0, sign), yDistance);
		SetCoordinates(nextCoordinates);

		tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(yDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		if (!pushed)
		{
			parentGame.OnMoveEnded();
		}
	}

	protected IEnumerator MoveYThenX(int xDistance, int yDistance, bool pushed)
	{
		///////////////////////////////
		// Y MOVEMENT
		///////////////////////////////

		int sign = Mathf.FloorToInt(Mathf.Sign(yDistance));
		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, yDistance);
		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(0, sign), yDistance);
		SetCoordinates(nextCoordinates);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(yDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		///////////////////////////////
		// X MOVEMENT
		///////////////////////////////

		sign = Mathf.FloorToInt(Mathf.Sign(xDistance));
		nextCoordinates = currentCoordinates + new IntVector2(xDistance, 0);
		convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(sign, 0), xDistance);
		SetCoordinates(nextCoordinates);
		
		tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(xDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		if (!pushed)
		{
			parentGame.OnMoveEnded();
		}
	}

	protected IEnumerator MoveX(int distance, int distFromPushingPiece, int numPushedPieces, bool pushed)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));
		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(distance, 0);
		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(sign, 0), distance);
		SetCoordinates(nextCoordinates);

		if (pushed)
		{
			int clampPushed = Mathf.Clamp(numPushedPieces - 1, 0, numPushedPieces + 1);
			int clampDist = Mathf.Clamp((distFromPushingPiece - 1) - clampPushed, 0, distFromPushingPiece + 1);
			float waitTime = Constants.I.PieceMoveTime * clampDist;
			yield return new WaitForSeconds(waitTime);
		}

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(distance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		if (!pushed)
		{
			parentGame.OnMoveEnded();
		}
	}

	protected IEnumerator MoveY(int distance, int distFromPushingPiece, int numPushedPieces, bool pushed)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));
		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, distance);
		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(0, sign), distance);
		SetCoordinates(nextCoordinates);

		if (pushed)
		{
			int clampPushed = Mathf.Clamp(numPushedPieces - 1, 0, numPushedPieces + 1);
			int clampDist = Mathf.Clamp((distFromPushingPiece - 1) - clampPushed, 0, distFromPushingPiece + 1);
			float waitTime = Constants.I.PieceMoveTime * clampDist;
			yield return new WaitForSeconds(waitTime);
		}

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(distance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		if (!pushed)
		{
			parentGame.OnMoveEnded();
		}
	}

	protected IEnumerator MoveDiagonally(int xDistance, int yDistance, int distFromPushingPiece, int numPushedPieces, bool pushed)
	{
		int xSign = Mathf.FloorToInt(Mathf.Sign(xDistance));
		int ySign = Mathf.FloorToInt(Mathf.Sign(yDistance));
		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(xDistance, yDistance);
		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		parentGame.OnPieceMove(this, new IntVector2(xSign, ySign), xDistance);
		SetCoordinates(nextCoordinates);

		if (pushed)
		{
			int clampPushed = Mathf.Clamp(numPushedPieces - 1, 0, numPushedPieces + 1);
			int clampDist = Mathf.Clamp((distFromPushingPiece - 1) - clampPushed, 0, distFromPushingPiece + 1);
			float waitTime = Constants.I.PieceMoveTime * clampDist;
			yield return new WaitForSeconds(waitTime);
		}

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(xDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		if (!pushed)
		{
			parentGame.OnMoveEnded();
		}
	}
}