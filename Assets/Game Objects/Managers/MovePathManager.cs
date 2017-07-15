using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePathManager : MonoBehaviour 
{
	public static MovePathManager Instance = null;

	/*
	[Header("Move Sprite Prefabs")]
	[SerializeField] private GameObject closed;

	[SerializeField] private GameObject corner_DL;
	[SerializeField] private GameObject corner_DR;
	[SerializeField] private GameObject corner_UL;
	[SerializeField] private GameObject corner_UR;

	[SerializeField] private GameObject end_D;
	[SerializeField] private GameObject end_L;
	[SerializeField] private GameObject end_R;
	[SerializeField] private GameObject end_U;
	[SerializeField] private GameObject end_DL;
	[SerializeField] private GameObject end_DR;
	[SerializeField] private GameObject end_UL;
	[SerializeField] private GameObject end_UR;

	[SerializeField] private GameObject passage_H;
	[SerializeField] private GameObject passage_V;
	[SerializeField] private GameObject passage_DL;
	[SerializeField] private GameObject passage_DR;
	[SerializeField] private GameObject passage_UL;
	[SerializeField] private GameObject passage_UR;

	// Stored list of move objects that are instantiated for easy cleanup.
	[SerializeField] private List<GameObject> moveObjects;

	// Stored list of "deleted" end objects for use when we need to reverse.
	[SerializeField] private List<GameObject> reverseObjects;

	private GameObject closedStartObj;
	private GameObject openStartObj;

	private Move mostRecentMove;

	[SerializeField] private IntVector2[] previousPath;
	*/

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

/*
	public void BeginPath(IntVector2 coords)
	{
		if (openStartObj != null && closedStartObj == null)
		{
			EndPath();
			closedStartObj = Instantiate(closed);
			closedStartObj.transform.position = GameManager.Instance.CoordinateToPosition(coords);
			closedStartObj.transform.parent = this.transform;
			startCoords = coords;
		}
		else if (openStartObj == null && closedStartObj == null)
		{
			closedStartObj = Instantiate(closed);
			closedStartObj.transform.position = GameManager.Instance.CoordinateToPosition(coords);
			closedStartObj.transform.parent = this.transform;
			startCoords = coords;
		}
	}

	public void DrawPath(Move move)
	{	
		// Handle reverse movements.
		if (mostRecentMove != null && 
			move.coordinates == mostRecentMove.reverse.coordinates && 
			move.coordinates != startCoords)
		{
			SpawnReverse();

			return;
		}

		moves.Add(move.moveOffset);

		if (move.moveOffset == new IntVector2(0, 1)) // up
		{
			SpawnStartObjectIfNeeded(end_D);
			SpawnPassageObjectIfNeeded(passage_V, move);
			SpawnEndObject(end_U, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(0, -1)) // down
		{
			SpawnStartObjectIfNeeded(end_U);
			SpawnPassageObjectIfNeeded(passage_V, move);
			SpawnEndObject(end_D, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(-1, 0)) // left
		{
			SpawnStartObjectIfNeeded(end_R);
			SpawnPassageObjectIfNeeded(passage_H, move);
			SpawnEndObject(end_L, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(1, 0)) // right
		{
			SpawnStartObjectIfNeeded(end_L);
			SpawnPassageObjectIfNeeded(passage_H, move);
			SpawnEndObject(end_R, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(-1, 1)) // up_left
		{
			SpawnStartObjectIfNeeded(end_DR);
			SpawnPassageObjectIfNeeded(passage_UL, move);
			SpawnEndObject(end_UL, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(1, 1)) // up_right
		{
			SpawnStartObjectIfNeeded(end_DL);
			SpawnPassageObjectIfNeeded(passage_UR, move);
			SpawnEndObject(end_UR, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(-1, -1)) // down_left
		{
			SpawnStartObjectIfNeeded(end_UR);
			SpawnPassageObjectIfNeeded(passage_DL, move);
			SpawnEndObject(end_DL, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(1, -1)) // down_right
		{
			SpawnStartObjectIfNeeded(end_UL);
			SpawnPassageObjectIfNeeded(passage_DR, move);
			SpawnEndObject(end_DR, move.coordinates);
		}

		mostRecentMove = move;
	}

	public void EndPath()
	{
		for (int i = 0; i < moveObjects.Count; i++)
		{
			Destroy(moveObjects[i].gameObject);
		}
		for (int i = 0; i < reverseObjects.Count; i++)
		{
			Destroy(reverseObjects[i].gameObject);
		}

		moveObjects.Clear();
		moves.Clear();
		reverseObjects.Clear();

		mostRecentMove = null;
		startCoords = IntVector2.NULL;

		if (closedStartObj != null)
		{
			Destroy(closedStartObj.gameObject);
		}

		if (openStartObj != null)
		{
			Destroy(openStartObj.gameObject);
		}
	}

	public void SpawnStartObjectIfNeeded(GameObject prefab)
	{
		// if this is the first movement we gotta spawn the different start piece.
		if (moves.Count == 1 && closedStartObj != null)
		{
			Destroy(closedStartObj.gameObject);

			openStartObj = Instantiate(prefab);
			openStartObj.transform.position = GameManager.Instance.CoordinateToPosition(startCoords);
			openStartObj.transform.parent = this.transform;
		}
	}

	public void SpawnPassageObjectIfNeeded(GameObject prefab, Move move)
	{
		if (moves.Count > 1 && move.reverse != null)
		{
			reverseObjects.Add(moveObjects[moveObjects.Count - 1]);
			moveObjects[moveObjects.Count - 1].SetActive(false);
			moveObjects.RemoveAt(moveObjects.Count - 1);

			GameObject obj = Instantiate(prefab);
			obj.transform.position = GameManager.Instance.CoordinateToPosition(move.reverse.coordinates);
			obj.transform.parent = this.transform;
			moveObjects.Add(obj);
		}
	}

	public void SpawnEndObject(GameObject prefab, IntVector2 coordinates)
	{
		GameObject obj = Instantiate(prefab);
		obj.transform.position = GameManager.Instance.CoordinateToPosition(coordinates);
		obj.transform.parent = this.transform;
		moveObjects.Add(obj);
	}

	public void SpawnReverse()
	{
		Destroy(moveObjects[moveObjects.Count - 1].gameObject); // Destroy end object.
		moveObjects.RemoveAt(moveObjects.Count - 1);
		Destroy(moveObjects[moveObjects.Count - 1].gameObject); // Destroy passage object.
		moveObjects.RemoveAt(moveObjects.Count - 1);

		moves.RemoveAt(moves.Count - 1);

		moveObjects.Add(reverseObjects[reverseObjects.Count - 1]);
		reverseObjects[reverseObjects.Count - 1].SetActive(true);
		reverseObjects.RemoveAt(reverseObjects.Count - 1);

		mostRecentMove = mostRecentMove.reverse;
	}

	public void SetColors(Piece piece)
	{

	}

	*/
}
