using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePathManager : MonoBehaviour 
{
	public static MovePathManager Instance = null;

	private IntVector2 startCoords;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		startCoords = IntVector2.NULL;
	}

	public void BeginPath(IntVector2 coords)
	{
		startCoords = coords;
	}

	// Done with every piece except the knight.
	public IntVector2[] CalculatePath(Move move)
	{
		if (startCoords == IntVector2.NULL || move.coordinates == startCoords) return null;

		IntVector2 diff = move.coordinates - startCoords;

		if (diff.x != 0 && diff.y == 0) // horizontal!
		{
			float sign = Mathf.Sign(diff.x);
			int count = Mathf.Abs(diff.x);

			IntVector2[] ret = new IntVector2[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = new IntVector2(startCoords.x + (int)((i+1) * sign), startCoords.y);
			}

			return ret;
		}
		else if (diff.x == 0 && diff.y != 0)
		{
			float sign = Mathf.Sign(diff.y);
			int count = Mathf.Abs(diff.y);

			IntVector2[] ret = new IntVector2[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = new IntVector2(startCoords.x, startCoords.y + (int)((i+1) * sign));
			}

			return ret;
		}
		else if (diff.x != 0 && diff.y != 0)
		{
			float signX = Mathf.Sign(diff.x);
			float signY = Mathf.Sign(diff.y);
			int count = Mathf.Abs(diff.x);

			IntVector2[] ret = new IntVector2[count];
			for (int i = 0; i < count; i++)
			{
				ret[i] = new IntVector2(startCoords.x + (int)((i+1) * signX), startCoords.y + (int)((i+1) * signY));
			}

			return ret;
		}
		else
		{
			return null;
		}
	}

	public IntVector2[] CalculateKnightPath(IntVector2[] path, Move move)
	{
		if (startCoords == IntVector2.NULL || move.coordinates == startCoords) return null;


		IntVector2[] ret = null;

		if (path != null)
		{
			// if reverse we delete the last item of the path.
			if (move.coordinates == path[path.Length - 1])
			{
				ret = new IntVector2[path.Length - 1];
				for (int i = 0; i < ret.Length; i++)
				{
					ret[i] = path[i];
				}
			}
			else
			{
				ret = new IntVector2[path.Length + 1];
				for (int i = 0; i < path.Length; i++)
				{
					ret[i] = path[i];
				}

				ret[ret.Length - 1] = new IntVector2(move.coordinates.x, move.coordinates.y);
			}
		}
		else
		{
			ret = new IntVector2[1];
			ret[0] = new IntVector2(move.coordinates.x, move.coordinates.y);
		}

		return ret;
	}

	public void EndPath()
	{
		startCoords = IntVector2.NULL;
	}
}
