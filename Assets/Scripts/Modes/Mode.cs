using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public abstract class Mode : MonoBehaviour 
{
	/////////////////////////////////////////////////////////////////////
	// CONSTANTS
	/////////////////////////////////////////////////////////////////////

	/////////////////////////////////////////////////////////////////////
	// PUBLICS
	/////////////////////////////////////////////////////////////////////

	public enum PieceType
	{
		KING,
		QUEEN,
		ROOK,
		BISHOP,
		KNIGHT,
		PAWN
	}

	/////////////////////////////////////////////////////////////////////
	// PRIVATES
	/////////////////////////////////////////////////////////////////////

	protected GameObject[,] tileObjects;
	protected Piece[,] pieces;
	protected PieceType nextRandomPieceType;

	[ReadOnly, SerializeField] protected Piece currentSelectedPiece;

	protected virtual void Awake()
	{
		tileObjects = new GameObject[Constants.instance.GRID_SIZE.x, Constants.instance.GRID_SIZE.y];
		pieces = new Piece[Constants.instance.GRID_SIZE.x, Constants.instance.GRID_SIZE.y];

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
				pieces[newCoordinates.x, newCoordinates.y].MoveTo(push, true);
			}
			else
			{
				// PUSH OFF THE GRID. WOOHOO. TEN POINTS. FIVE POINTS. YOU WIN.
				Debug.Log("YOU JUST PUSHED A PIECE OFF THE GRID");
				PieceOffGrid(pieces[newCoordinates.x, newCoordinates.y], push);
				pieces[newCoordinates.x, newCoordinates.y] = null;
			}
		}
		else
		{
			Debug.Log("Normal Move. No Push");
		}
	}

	protected virtual void PieceOffGrid(Piece piece, IntVector2 pushCoordinates)
	{
		Vector2 offGridPosition = GameManager.Instance.CoordinateToPosition(pushCoordinates);
		StartCoroutine(MovePieceOffGrid(piece, offGridPosition));
		PlaceRandomPiece();
	}

	protected IEnumerator MovePieceOffGrid(Piece piece, Vector2 position)
	{
		GameObject pieceObj = piece.gameObject;

		Vector3 newPos = new Vector3(position.x, position.y, pieceObj.transform.position.z);

		Tween tween = pieceObj.transform.DOMove(newPos, Constants.instance.PIECE_MOVE_TIME);
		yield return tween.WaitForCompletion();

		piece.SetSortingOrder(-11);
		Destroy(piece); // Destroy the piece class so it won't get touched.

		tween = pieceObj.transform.DOMoveY(-10f, 1f)
			.SetEase(Ease.InCubic);
		yield return tween.WaitForCompletion();

		Destroy(pieceObj);
	}

	public void UpdatePieceCoordinates(Piece piece, IntVector2 oldCoordinates, IntVector2 newCoordinates)
	{
		pieces[oldCoordinates.x, oldCoordinates.y] = null;
		pieces[newCoordinates.x, newCoordinates.y] = piece;
	}

	public bool IsWithinBounds(IntVector2 coordinates)
	{
		return (coordinates.x >= 0 && coordinates.x < Constants.instance.GRID_SIZE.x) && (coordinates.y >= 0 && coordinates.y < Constants.instance.GRID_SIZE.y);
	}

	public virtual void Load() {}
	public virtual void Unload() {}

	public void ResetPossibleMoves()
	{
		for (int i = 0; i < tileObjects.GetLength(0); i++)
		{
			for (int j = 0; j < tileObjects.GetLength(1); j++)
			{
				tileObjects[i, j].GetComponent<Tile>().HideMove();

				if (pieces[i, j] != null)
				{
					pieces[i, j].SetPushPotential(false);
				}
			}
		}
	}

	public virtual void OnMoveInitiated()
	{

	}

	public virtual void OnMoveEnded()
	{

	}

	IntVector2 RandomCoordinates()
	{
		return new IntVector2(Random.Range(0, Constants.instance.GRID_SIZE.x), Random.Range(0, Constants.instance.GRID_SIZE.y));
	}

	protected void PlaceRandomPiece()
	{
		int rand = Random.Range(0, 6);
		IntVector2 randCoords = RandomCoordinates();

		while (pieces[randCoords.x, randCoords.y] != null)
		{
			randCoords = RandomCoordinates();
		}

		switch (rand)
		{
			case 0:
				CreateKing(randCoords.x, randCoords.y);
				break;
			case 1:
				CreateQueen(randCoords.x, randCoords.y);
				break;
			case 2:
				CreateRook(randCoords.x, randCoords.y);
				break;
			case 3:
				CreateBishop(randCoords.x, randCoords.y);
				break;
			case 4:
				CreateKnight(randCoords.x, randCoords.y);
				break;
			case 5:
				CreatePawn(randCoords.x, randCoords.y);
				break;
			default:
				break;
		}
	}

	protected virtual void DecideNextRandomPiece()
	{
		int rand = Random.Range(0, 6);

		switch (rand)
		{
			case 0:
				nextRandomPieceType = PieceType.KING;
				break;
			case 1:
				nextRandomPieceType = PieceType.QUEEN;
				break;
			case 2:
				nextRandomPieceType = PieceType.ROOK;
				break;
			case 3:
				nextRandomPieceType = PieceType.BISHOP;
				break;
			case 4:
				nextRandomPieceType = PieceType.KNIGHT;
				break;
			case 5:
				nextRandomPieceType = PieceType.PAWN;
				break;
			default:
				break;
		}
	}

	protected void PlaceNextRandomPiece()
	{
		IntVector2 randCoords = RandomCoordinates();

		while (pieces[randCoords.x, randCoords.y] != null)
		{
			randCoords = RandomCoordinates();
		}

		switch (nextRandomPieceType)
		{
			case PieceType.KING:
				CreateKing(randCoords.x, randCoords.y);
				break;
			case PieceType.QUEEN:
				CreateQueen(randCoords.x, randCoords.y);
				break;
			case PieceType.ROOK:
				CreateRook(randCoords.x, randCoords.y);
				break;
			case PieceType.BISHOP:
				CreateBishop(randCoords.x, randCoords.y);
				break;
			case PieceType.KNIGHT:
				CreateKnight(randCoords.x, randCoords.y);
				break;
			case PieceType.PAWN:
				CreatePawn(randCoords.x, randCoords.y);
				break;
			default:
				break;
		}

		DecideNextRandomPiece();
	}

	protected virtual void PlacePieces()
	{
		CreatePawn(1, 1);
		CreatePawn(2, 1);
		CreatePawn(3, 1);
		CreateKnight(1, 0);
		CreateRook(2, 0);
		CreateKnight(3, 0);
	}

	protected void CreateKing(int x, int y)
	{
		GameObject kingObj = Instantiate(GameManager.Instance.kingPrefab) as GameObject;
		King king = kingObj.GetComponent<King>();
		SetupPiece(king, "King", x, y);
	}

	protected void CreateQueen(int x, int y)
	{
		GameObject queenObj = Instantiate(GameManager.Instance.queenPrefab) as GameObject;
		Queen queen = queenObj.GetComponent<Queen>();
		SetupPiece(queen, "Queen", x, y);
	}

	protected void CreateRook(int x, int y)
	{
		GameObject rookObj = Instantiate(GameManager.Instance.rookPrefab) as GameObject;
		Rook rook = rookObj.GetComponent<Rook>();
		SetupPiece(rook, "Rook", x, y);
	}

	protected void CreateBishop(int x, int y)
	{
		GameObject bishopObj = Instantiate(GameManager.Instance.bishopPrefab) as GameObject;
		Bishop bishop = bishopObj.GetComponent<Bishop>();
		SetupPiece(bishop, "Bishop", x, y);
	}

	protected void CreateKnight(int x, int y)
	{
		GameObject knightObj = Instantiate(GameManager.Instance.knightPrefab) as GameObject;
		Knight knight = knightObj.GetComponent<Knight>();
		SetupPiece(knight, "Knight", x, y);
	}

	protected void CreatePawn(int x, int y) 
	{
		GameObject pawnObj = Instantiate(GameManager.Instance.pawnPrefab) as GameObject;
		Pawn pawn = pawnObj.GetComponent<Pawn>();
		SetupPiece(pawn, "Pawn", x, y);
	}

	void SetupPiece(Piece piece, string name, int x, int y)
	{
		piece.gameObject.name = name;
		piece.transform.parent = transform;
		piece.transform.position = new Vector3(tileObjects[x, y].transform.position.x, tileObjects[x, y].transform.position.y, piece.transform.position.z);
		piece.SetInfo(x, y, this);

		pieces[x, y] = piece;
	}

	// Returns a list of coordinates diagonal to the given coordinates that contain pieces.
	public List<IntVector2> GetDiagonalPieceCoordinates(IntVector2 coords)
	{
		List<IntVector2> retList = new List<IntVector2>();

		IntVector2 upRight = new IntVector2(coords.x + 1, coords.y + 1);
		IntVector2 downRight = new IntVector2(coords.x + 1, coords.y - 1);
		IntVector2 downLeft = new IntVector2(coords.x - 1, coords.y - 1);
		IntVector2 upLeft = new IntVector2(coords.x - 1, coords.y + 1);

		if (IsWithinBounds(upRight) && pieces[upRight.x, upRight.y] != null)
		{
			retList.Add(upRight);
		}

		if (IsWithinBounds(downRight) && pieces[downRight.x, downRight.y] != null)
		{
			retList.Add(downRight);
		}

		if (IsWithinBounds(downLeft) && pieces[downLeft.x, downLeft.y] != null)
		{
			retList.Add(downLeft);
		}

		if (IsWithinBounds(upLeft) && pieces[upLeft.x, upLeft.y] != null)
		{
			retList.Add(upLeft);
		}

		return retList;
	}
}













