using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Move
{
	public IntVector2 coordinates; // The coordinates of wherever this move is.
	public IntVector2 moveOffset; // The offset needed to be applied to get to the current coords.

	public Move reverse = null; // The last move before this one, set on the fly for undoing moves and going back to the prev.
	public bool isFinal = false; // This is the last move in a possible piece movement. If we lift finger, we can move here.

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
	[SerializeField] Color tint = Color.white;
	[SerializeField] Color disabledTint = Color.black;
	[ReadOnly] public bool potentialPush = false;

	protected InitialMove moveset;

	// Array of coordinate offsets that define the moves a piece can make.
	protected IntVector2[] moveOffsets;

	protected int moveMagnitude = 1;
	protected IntVector2 currentCoordinates;
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
		if (hasPotential)
		{
			PickPieceUp();
		}
		else
		{
			SetPieceDown();
		}

		potentialPush = hasPotential;
	}

	// For when it's underneath a tile that's highlighted.
	void PickPieceUp()
	{
		sprite.transform.localPosition += Vector3.up * 0.1f;
	}

	void SetPieceDown()
	{
		sprite.transform.localPosition = Vector3.zero;
	}

	void OnEnable()
	{
		// Spawn in 1 second after the tiles do.
		GameManager.Instance.GrowMe(this.gameObject);
	}

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

	public virtual void DetermineMoveset()
	{

	}

	protected MoveUp GetUp(InitialMove init)
	{
		IntVector2 offset = new IntVector2(0, 1);
		IntVector2 newCoords = currentCoordinates + offset;

		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveUp up = new MoveUp();
			up.coordinates = newCoords;
			up.moveOffset = offset;
			up.reverse = init;
			return up;
		}
		else
		{
			return null;
		}
	}

	protected MoveDown GetDown(InitialMove init)
	{
		IntVector2 offset = new IntVector2(0, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveDown down = new MoveDown();
			down.coordinates = newCoords;
			down.moveOffset = offset;
			down.reverse = init;
			return down;
		}
		else
		{
			return null;
		}
	}

	protected MoveLeft GetLeft(InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, 0);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveLeft left = new MoveLeft();
			left.coordinates = newCoords;
			left.moveOffset = offset;
			left.reverse = init;
			return left;
		}
		else
		{
			return null;
		}
	}

	protected MoveRight GetRight(InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, 0);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveRight right = new MoveRight();
			right.coordinates = newCoords;
			right.moveOffset = offset;
			right.reverse = init;
			return right;
		}
		else
		{
			return null;
		}
	}

	protected virtual MoveUpRight GetUpRight(InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveUpRight upRight = new MoveUpRight();
			upRight.coordinates = newCoords;
			upRight.moveOffset = offset;
			upRight.reverse = init;
			return upRight;
		}
		else
		{
			return null;
		}
	}

	protected virtual MoveDownRight GetDownRight(InitialMove init)
	{
		IntVector2 offset = new IntVector2(1, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveDownRight downRight = new MoveDownRight();
			downRight.coordinates = newCoords;
			downRight.moveOffset = offset;
			downRight.reverse = init;
			return downRight;
		}
		else
		{
			return null;
		}
	}

	protected virtual MoveDownLeft GetDownLeft(InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, -1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveDownLeft downLeft = new MoveDownLeft();
			downLeft.coordinates = newCoords;
			downLeft.moveOffset = offset;
			downLeft.reverse = init;
			return downLeft;
		}
		else
		{
			return null;
		}
	}

	protected virtual MoveUpLeft GetUpLeft(InitialMove init)
	{
		IntVector2 offset = new IntVector2(-1, 1);
		IntVector2 newCoords = currentCoordinates + offset;
		if (parentGame.IsWithinBounds(newCoords))
		{
			MoveUpLeft upLeft = new MoveUpLeft();
			upLeft.coordinates = newCoords;
			upLeft.moveOffset = offset;
			upLeft.reverse = init;
			return upLeft;
		}
		else
		{
			return null;
		}
	}

	public virtual void MoveTo(IntVector2 coordinates, bool pushed)
	{
		// This is the core disable / pushing logic.
		if (pushed)
		{
			if (moveDisabled)
			{
				SetMoveDisabled(false);
			}
		}
		else
		{
			SetMoveDisabled(true);
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
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
		{
			// Move horizontally first, then vertically.
			StartCoroutine(MoveXThenY(diff.x, diff.y));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) < Mathf.Abs(diff.y))
		{
			// Move vertically first, then horizontally.
			StartCoroutine(MoveYThenX(diff.x, diff.y));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) == Mathf.Abs(diff.y))
		{
			// Move diagonally.
			StartCoroutine(MoveDiagonally(diff.x, diff.y));
		}
	}

	public bool GetMoveDisabled()
	{
		return moveDisabled;
	}

	protected void SetMoveDisabled(bool disabled)
	{
		moveDisabled = disabled;
		if (disabled)
		{
			sprite.color = disabledTint;
		}
		else
		{
			sprite.color = tint;
		}
	}

	protected IEnumerator MoveXThenY(int xDistance, int yDistance)
	{
		///////////////////////////////
		// X MOVEMENT
		///////////////////////////////

		int sign = Mathf.FloorToInt(Mathf.Sign(xDistance));

		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(xDistance, 0);
		parentGame.OnPieceMove(this, new IntVector2(sign, 0), xDistance);

		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(xDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		///////////////////////////////
		// Y MOVEMENT
		///////////////////////////////

		sign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		nextCoordinates = currentCoordinates + new IntVector2(0, yDistance);
		parentGame.OnPieceMove(this, new IntVector2(0, sign), yDistance);

		convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(yDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveYThenX(int xDistance, int yDistance)
	{
		///////////////////////////////
		// Y MOVEMENT
		///////////////////////////////

		int sign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, yDistance);
		parentGame.OnPieceMove(this, new IntVector2(0, sign), yDistance);

		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(yDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		parentGame.OnMoveEnded();

		///////////////////////////////
		// X MOVEMENT
		///////////////////////////////

		sign = Mathf.FloorToInt(Mathf.Sign(xDistance));

		nextCoordinates = currentCoordinates + new IntVector2(xDistance, 0);
		parentGame.OnPieceMove(this, new IntVector2(sign, 0), xDistance);

		convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(xDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveX(int distance)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));

		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(distance, 0);
		parentGame.OnPieceMove(this, new IntVector2(sign, 0), distance);

		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(distance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveY(int distance)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));

		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, distance);
		parentGame.OnPieceMove(this, new IntVector2(0, sign), distance);

		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(distance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveDiagonally(int xDistance, int yDistance)
	{
		int xSign = Mathf.FloorToInt(Mathf.Sign(xDistance));
		int ySign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		IntVector2 nextCoordinates = currentCoordinates + new IntVector2(xDistance, yDistance);
		parentGame.OnPieceMove(this, new IntVector2(xSign, ySign), xDistance);

		Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
		Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

		Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime * Mathf.Abs(xDistance)).SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		SetCoordinates(nextCoordinates);

		parentGame.OnMoveEnded();
	}
}

















