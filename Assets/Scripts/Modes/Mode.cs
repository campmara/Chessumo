using UnityEngine;
using System.Collections;

public abstract class Mode : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// CONSTANTS
	/////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////
	// PUBLICS
	/////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////

	public IntVector2 GridSize { get { return gridSize; } }
	[SerializeField] protected IntVector2 gridSize = new IntVector2(5, 5);

	protected GameObject[,] tileObjects;
	protected Piece[,] pieces;

	[ReadOnly, SerializeField] protected Piece currentSelectedPiece;

	protected virtual void Awake()
	{
		tileObjects = new GameObject[gridSize.x, gridSize.y];
		pieces = new Piece[gridSize.x, gridSize.y];
		currentSelectedPiece = null;
	}

	public void OnPieceMove(Piece piece, IntVector2 newCoordinates)
	{
		IntVector2 oldCoordinates = piece.GetCoordinates();

		if (pieces[newCoordinates.x, newCoordinates.y] != null)
		{
			// We're pushing! Yay!
			Debug.Log("PUSHING PIECE");

			IntVector2 push = newCoordinates + (newCoordinates - oldCoordinates);

			if (IsWithinBounds(push))
			{
				// Push in the grid.
				Debug.Log("Pushing within bounds.");
				pieces[newCoordinates.x, newCoordinates.y].MoveToTile(tileObjects[push.x, push.y].GetComponent<Tile>());
			}
			else
			{
				// PUSH OFF THE GRID. WOOHOO. TEN POINTS. FIVE POINTS. YOU WIN.
				Debug.Log("YOU JUST PUSHED A PIECE OFF THE GRID");
				Destroy(pieces[newCoordinates.x, newCoordinates.y].gameObject);
				pieces[newCoordinates.x, newCoordinates.y] = null;
			}
		}
		else
		{
			Debug.Log("Normal Move. No Push");
		}
	}

	public void UpdatePieceCoordinates(Piece piece, IntVector2 oldCoordinates, IntVector2 newCoordinates)
	{
		pieces[oldCoordinates.x, oldCoordinates.y] = null;
		pieces[newCoordinates.x, newCoordinates.y] = piece;
	}

	public bool IsWithinBounds(IntVector2 coordinates)
	{
		return (coordinates.x >= 0 && coordinates.x < gridSize.x) && (coordinates.y >= 0 && coordinates.y < gridSize.y);
	}

	public virtual void Load() {}
	public virtual void Unload() {}
}













