using UnityEngine;
using System.Collections;

public abstract class Piece : MonoBehaviour 
{
	[ReadOnly] public bool potentialPush = false;

	const float MOVE_LERP_TIME = 0.25f;

	// Array of coordinate offsets that define the moves a piece can make.
	protected IntVector2[] moveOffsets;
	protected int moveMagnitude = 1;
	protected IntVector2 currentCoordinates;
	protected Mode parentMode;

	public IntVector2 GetCoordinates()
	{
		return currentCoordinates;
	}

	public void SetCoordinates(IntVector2 coordinates)
	{
		parentMode.UpdatePieceCoordinates(this, currentCoordinates, coordinates);
		currentCoordinates = coordinates;
	}

	public void SetInfo(int x, int y, Mode mode)
	{
		currentCoordinates = new IntVector2(x, y);
		parentMode = mode;
	}

	public IntVector2[] GetPossibleMoves()
	{
		IntVector2[] returnArray = new IntVector2[moveOffsets.Length * moveMagnitude];

		for (int i = 0; i < returnArray.Length; i++)
		{
			int offsetIndex = Mathf.FloorToInt(i / moveMagnitude);
			int coefficient = moveMagnitude > 1 ? (i + 1) % moveMagnitude : 1;
			IntVector2 moveOffset = new IntVector2(moveOffsets[offsetIndex].x * coefficient, moveOffsets[offsetIndex].y * coefficient);
			//Debug.Log("Move Offset: [" + moveOffset.x + ", " + moveOffset.y + "]");

			returnArray[i] = currentCoordinates + moveOffset;
		}

		return returnArray;
	}

	public void MoveToTile(Tile tile)
	{
		// Determine how many times we must repeat the movement to get to the desired point.
		IntVector2 diff = tile.GetCoordinates() - GetCoordinates();

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

	IEnumerator MoveXThenY(int xDistance, int yDistance)
	{
		yield return StartCoroutine(MoveX(xDistance));
		StartCoroutine(MoveY(yDistance));
	}

	IEnumerator MoveYThenX(int xDistance, int yDistance)
	{
		yield return StartCoroutine(MoveY(yDistance));
		StartCoroutine(MoveX(xDistance));
	}

	IEnumerator MoveX(int distance)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));

		for (int i = 0; i < Mathf.Abs(distance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(sign, 0);

			// Send Information to the pieces that are affected by the move.
			parentMode.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 oldPos = transform.position;
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			float timer = 0f;

			while (timer < MOVE_LERP_TIME)
			{
				timer += Time.deltaTime;
				transform.position = Vector3.Lerp(oldPos, newPos, timer / MOVE_LERP_TIME);
				yield return null;
			}
			
			SetCoordinates(nextCoordinates);

			yield return null;
		}
	}

	IEnumerator MoveY(int distance)
	{
		int sign = Mathf.FloorToInt(Mathf.Sign(distance));

		for (int i = 0; i < Mathf.Abs(distance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(0, sign);

			// Send Information to the pieces that are affected by the move.
			parentMode.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 oldPos = transform.position;
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			float timer = 0f;

			while (timer < MOVE_LERP_TIME)
			{
				timer += Time.deltaTime;
				transform.position = Vector3.Lerp(oldPos, newPos, timer / MOVE_LERP_TIME);
				yield return null;
			}

			SetCoordinates(nextCoordinates);

			yield return null;
		}
	}

	IEnumerator MoveDiagonally(int xDistance, int yDistance)
	{
		int xSign = Mathf.FloorToInt(Mathf.Sign(xDistance));
		int ySign = Mathf.FloorToInt(Mathf.Sign(yDistance));

		for (int i = 0; i < Mathf.Abs(xDistance); i++)
		{
			IntVector2 nextCoordinates = currentCoordinates + new IntVector2(xSign, ySign);

			// Send Information to the pieces that are affected by the move.
			parentMode.OnPieceMove(this, nextCoordinates);

			Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
			Vector3 oldPos = transform.position;
			Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);

			float timer = 0f;

			while (timer < MOVE_LERP_TIME)
			{
				timer += Time.deltaTime;
				transform.position = Vector3.Lerp(oldPos, newPos, timer / MOVE_LERP_TIME);
				yield return null;
			}

			SetCoordinates(nextCoordinates);

			yield return null;
		}
	}
}

















