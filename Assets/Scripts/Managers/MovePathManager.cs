using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePathManager : MonoBehaviour 
{
	public static MovePathManager Instance = null;

	[Header("Move Sprite Prefabs")]
	[SerializeField] private GameObject moveHorizontal;
	[SerializeField] private GameObject moveVertical;

	[SerializeField] private GameObject moveDiagonal_BL2TR;
	[SerializeField] private GameObject moveDiagonal_TL2BR;

	[SerializeField] private GameObject moveCorner_DLRU;
	[SerializeField] private GameObject moveCorner_DRLU;
	[SerializeField] private GameObject moveCorner_ULRD;
	[SerializeField] private GameObject moveCorner_URLD;

	[SerializeField] private GameObject moveEnd_Down;
	[SerializeField] private GameObject moveEnd_Left;
	[SerializeField] private GameObject moveEnd_Right;
	[SerializeField] private GameObject moveEnd_Up;
	[SerializeField] private GameObject moveEnd_UR;
	[SerializeField] private GameObject moveEnd_UL;
	[SerializeField] private GameObject moveEnd_DL;
	[SerializeField] private GameObject moveEnd_DR;

	// A sequential list of move offsets. When we move the knight we'll get the first move from here.
	// This will also help with determining which arrow sprite to use.
	private List<IntVector2> moves;

	// Stored list of move objects that are instantiated for easy cleanup.
	private List<GameObject> moveObjects;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		moves = new List<IntVector2>();
		moveObjects = new List<GameObject>();
	}

	public void AddMove(IntVector2 coords, IntVector2 move)
	{
		moves.Add(move);

		GameObject obj;

		// Place move object. Determine which one to spawn based on various factors, looking back up to 2 moves.

		if (move.x > 0 && move.y == 0)
		{
			obj = Instantiate(moveEnd_Right) as GameObject;
		}
		else if (move.x < 0 && move.y == 0)
		{
			obj = Instantiate(moveEnd_Left) as GameObject;
		}
		else if (move.x == 0 && move.y > 0)
		{
			obj = Instantiate(moveEnd_Up) as GameObject;
		}
		else if (move.x == 0 && move.y < 0)
		{
			obj = Instantiate(moveEnd_Down) as GameObject;
		}
		else if (move.x > 0 && move.y > 0)
		{
			obj = Instantiate(moveEnd_UR) as GameObject;
		}
		else if (move.x < 0 && move.y > 0)
		{
			obj = Instantiate(moveEnd_UL) as GameObject;
		}
		else if (move.x < 0 && move.y < 0)
		{
			obj = Instantiate(moveEnd_DL) as GameObject;
		}
		else if (move.x > 0 && move.y < 0)
		{
			obj = Instantiate(moveEnd_DR) as GameObject;
		}
		else
		{
			obj = Instantiate(moveHorizontal) as GameObject;
		}

		obj.transform.position = GameManager.Instance.CoordinateToPosition(coords);
		obj.transform.parent = this.transform;
		moveObjects.Add(obj);
	}

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

	public void Reset()
	{
		for (int i = 0; i < moveObjects.Count; i++)
		{
			Destroy(moveObjects[i].gameObject);
		}
		moveObjects.Clear();
		moves.Clear();
	}
}
