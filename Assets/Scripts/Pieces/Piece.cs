using UnityEngine;
using System.Collections;
using DG.Tweening;

public abstract class Piece : MonoBehaviour 
{
	[SerializeField] Color tint = Color.white;
	[SerializeField] Color disabledTint = Color.black;
	[ReadOnly] public bool potentialPush = false;

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

		Coroutine moveRoutine = null;

		if (diff.x != 0 && diff.y == 0)
		{
			// Move horizontally.
			moveRoutine = StartCoroutine(MoveX(diff.x));
		}
		else if (diff.y != 0 && diff.x == 0)
		{
			// Move vertically.
			moveRoutine = StartCoroutine(MoveY(diff.y));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
		{
			// Move horizontally first, then vertically.
			moveRoutine = StartCoroutine(MoveXThenY(diff.x, diff.y));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) < Mathf.Abs(diff.y))
		{
			// Move vertically first, then horizontally.
			moveRoutine = StartCoroutine(MoveYThenX(diff.x, diff.y));
		}
		else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) == Mathf.Abs(diff.y))
		{
			// Move diagonally.
			moveRoutine = StartCoroutine(MoveDiagonally(diff.x, diff.y));
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

		for (int i = 0; i < Mathf.Abs(xDistance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(sign, 0);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		///////////////////////////////
		// Y MOVEMENT
		///////////////////////////////

		sign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		for (int i = 0; i < Mathf.Abs(yDistance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, sign);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		// End Move Callback
		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveYThenX(int xDistance, int yDistance)
	{
		///////////////////////////////
		// Y MOVEMENT
		///////////////////////////////

		int sign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		for (int i = 0; i < Mathf.Abs(yDistance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, sign);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		///////////////////////////////
		// X MOVEMENT
		///////////////////////////////

		sign = Mathf.FloorToInt(Mathf.Sign(xDistance));

		for (int i = 0; i < Mathf.Abs(xDistance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(sign, 0);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		// End Move Callback
		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveX(int distance)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));

		for (int i = 0; i < Mathf.Abs(distance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(sign, 0);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		// End Move Callback
		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveY(int distance)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));

		for (int i = 0; i < Mathf.Abs(distance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, sign);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		// End Move Callback
		parentGame.OnMoveEnded();
	}

	protected IEnumerator MoveDiagonally(int xDistance, int yDistance)
	{
		int xSign = Mathf.FloorToInt(Mathf.Sign(xDistance));
		int ySign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		for (int i = 0; i < Mathf.Abs(xDistance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(xSign, ySign);

			// Send Information to the pieces that are affected by the move.
			parentGame.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			Tween tween = transform.DOMove(newPos, Constants.I.PieceMoveTime).SetEase(Ease.Linear);
			yield return tween.WaitForCompletion();

			SetCoordinates(nextCoordinates);

			yield return null;
		}

		// End Move Callback
		parentGame.OnMoveEnded();
	}
}

















