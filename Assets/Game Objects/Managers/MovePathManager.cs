using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePathManager : MonoBehaviour 
{
	public static MovePathManager Instance = null;

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

	// A sequential list of move offsets. When we move the knight we'll get the first move from here.
	// This will also help with determining which arrow sprite to use.
	private List<IntVector2> moves;

	// Stored list of move objects that are instantiated for easy cleanup.
	private List<GameObject> moveObjects;

	private GameObject closedStartObj;

	private Move mostRecentMove;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		
		moves = new List<IntVector2>();
		moveObjects = new List<GameObject>();
		mostRecentMove = null;
	}

	public void BeginPath(IntVector2 coords, bool isReturn)
	{
		if (isReturn && closedStartObj == null)
		{
			EndPath();
			closedStartObj = Instantiate(closed);
			closedStartObj.transform.position = GameManager.Instance.CoordinateToPosition(coords);
			closedStartObj.transform.parent = this.transform;
		}
		else if (!isReturn)
		{
			closedStartObj = Instantiate(closed);
			closedStartObj.transform.position = GameManager.Instance.CoordinateToPosition(coords);
			closedStartObj.transform.parent = this.transform;
		}
	}

	public void DrawPath(Move move)
	{
		// Get rid of the beginning path object.
		Destroy(closedStartObj.gameObject);

		// check if it's a reverse
		if (mostRecentMove != null && move.coordinates == mostRecentMove.reverse.coordinates)
		{
			// Is a reverse.
			Destroy(moveObjects[moveObjects.Count - 1].gameObject);
			moveObjects.RemoveAt(moveObjects.Count - 1);
			moveObjects.TrimExcess();
			moves.RemoveAt(moves.Count - 1);
			moves.TrimExcess();
			move = mostRecentMove.reverse;
		}

		moves.Add(move.moveOffset);

		if (move.moveOffset == new IntVector2(0, 1)) // up
		{
			SpawnEndObject(end_U, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(0, -1)) // down
		{
			SpawnEndObject(end_D, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(-1, 0)) // left
		{
			SpawnEndObject(end_L, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(1, 0)) // right
		{
			SpawnEndObject(end_R, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(-1, 1)) // up_left
		{
			SpawnEndObject(end_UL, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(1, 1)) // up_right
		{
			SpawnEndObject(end_UR, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(-1, -1)) // down_left
		{
			SpawnEndObject(end_DL, move.coordinates);
		}
		else if (move.moveOffset == new IntVector2(1, -1)) // down_right
		{
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
		moveObjects.Clear();
		moves.Clear();

		if (closedStartObj != null)
		{
			Destroy(closedStartObj.gameObject);
		}
	}

	public void SpawnEndObject(GameObject prefab, IntVector2 coordinates)
	{
		GameObject obj = Instantiate(prefab);
		obj.transform.position = GameManager.Instance.CoordinateToPosition(coordinates);
		obj.transform.parent = this.transform;
		moveObjects.Add(obj);
	}

/*
	public void AddMove(IntVector2 coords, IntVector2 move)
	{
		moves.Add(move);

		GameObject obj = DetermineArrow(move);
		obj.transform.position = GameManager.Instance.CoordinateToPosition(coords);
		obj.transform.parent = this.transform;
		moveObjects.Add(obj);

		// Now update the previous move to something it needs to be.
		if (moves.Count > 1)
		{
			GameObject newPrevObj = DeterminePreviousMove();
			newPrevObj.transform.position = moveObjects[moveObjects.Count - 2].gameObject.transform.position;
			newPrevObj.transform.parent = this.transform;

			Destroy(moveObjects[moveObjects.Count - 2].gameObject);
			moveObjects[moveObjects.Count - 2] = newPrevObj;
		}
	}

	private GameObject DetermineArrow(IntVector2 move)
	{
		GameObject obj;

		// Place move object. Determine which one to spawn based on various factors, looking back up to 2 moves.
		if (move.x > 0 && move.y == 0)
		{
			obj = Instantiate(moveEnd_Right) as GameObject;
		}
		else if (move.x > 0 && move.y < 0)
		{
			obj = Instantiate(moveEnd_DR) as GameObject;
		}
		else if (move.x > 0 && move.y > 0)
		{
			obj = Instantiate(moveEnd_UR) as GameObject;
		}
		else if (move.x < 0 && move.y == 0)
		{
			obj = Instantiate(moveEnd_Left) as GameObject;
		}
		else if (move.x < 0 && move.y > 0)
		{
			obj = Instantiate(moveEnd_UL) as GameObject;
		}
		else if (move.x < 0 && move.y < 0)
		{
			obj = Instantiate(moveEnd_DL) as GameObject;
		}
		else if (move.x == 0 && move.y > 0)
		{
			obj = Instantiate(moveEnd_Up) as GameObject;
		}
		else if (move.x == 0 && move.y < 0)
		{
			obj = Instantiate(moveEnd_Down) as GameObject;
		}
		else
		{
			Debug.LogError("Invalid Move Path");
			obj = Instantiate(moveHorizontal) as GameObject;
		}

		return obj;
	}

	private GameObject DeterminePreviousMove()
	{
		int index = moves.Count - 2;
		IntVector2 prevMove = moves[index];

		GameObject obj;
		
		if ((prevMove.x > 0 && prevMove.y == 0) || (prevMove.x < 0 && prevMove.y == 0))
		{
			obj = Instantiate(moveHorizontal);
		}
		else if ((prevMove.x > 0 && prevMove.y < 0) || (prevMove.x < 0 && prevMove.y > 0))
		{
			obj = Instantiate(moveDiagonal_TL2BR);
		}
		else if ((prevMove.x > 0 && prevMove.y > 0) || (prevMove.x < 0 && prevMove.y < 0))
		{
			obj = Instantiate(moveDiagonal_BL2TR);
		}
		else if ((prevMove.x == 0 && prevMove.y > 0) || (prevMove.x == 0 && prevMove.y < 0))
		{
			obj = Instantiate(moveVertical);
		}
		else
		{
			Debug.LogError("Invalid Move Path");
			obj = Instantiate(moveCorner_DLRU);
		}

		return obj;
	}
*/

	public IntVector2 GetFirstMoveDirection()
	{
		return moves[0];
	}

	public IntVector2 GetLastMoveDirection()
	{
		return moves[moves.Count - 1];
	}

	public int MoveCount()
	{
		return moves.Count;
	}
}
