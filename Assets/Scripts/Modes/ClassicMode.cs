using UnityEngine;
using System.Collections;

public class ClassicMode : Mode 
{
	//////////////////////////////////////////////////////////
	// CONSTANTS
	//////////////////////////////////////////////////////////



	//////////////////////////////////////////////////////////
	// PUBLICS
	//////////////////////////////////////////////////////////



	//////////////////////////////////////////////////////////
	// PRIVATES
	//////////////////////////////////////////////////////////



	//////////////////////////////////////////////////////////
	// ACTIVATION
	//////////////////////////////////////////////////////////

	/*
	override void Awake()
	{
		base.Awake();
	}
	*/

	public override void Load()
	{
		SetupPlayfield();
		PlacePieces();
	}

	void SetupPlayfield()
	{
		for (int x = 0; x < gridSize.x; x++)
		{
			for (int y = 0; y < gridSize.y; y++)
			{
				GameObject tileObj = Instantiate(GameManager.Instance.tilePrefab) as GameObject;
				Tile tile = tileObj.GetComponent<Tile>();
				tile.gameObject.name = "Tile (" + x + ", " + y + ")";
				tile.transform.parent = transform;

				tile.transform.position = GameManager.Instance.CoordinateToPosition(new IntVector2(x, y));

				tile.SetInfo(x, y, this);

				tileObjects[x, y] = tileObj;
			}
		}
	}

	void PlacePieces()
	{
		CreateKing(1, 0);
		CreateQueen(2, 0);
		CreateRook(3, 0);
		CreateBishop(1, 1);
		CreateKnight(2, 1);
		CreatePawn(3, 1);
	}

	void CreateKing(int x, int y)
	{
		GameObject kingObj = Instantiate(GameManager.Instance.kingPrefab) as GameObject;
		King king = kingObj.GetComponent<King>();
		SetupPiece(king, "King", x, y);
	}

	void CreateQueen(int x, int y)
	{
		GameObject queenObj = Instantiate(GameManager.Instance.queenPrefab) as GameObject;
		Queen queen = queenObj.GetComponent<Queen>();
		SetupPiece(queen, "Queen", x, y);
	}

	void CreateRook(int x, int y)
	{
		GameObject rookObj = Instantiate(GameManager.Instance.rookPrefab) as GameObject;
		Rook rook = rookObj.GetComponent<Rook>();
		SetupPiece(rook, "Rook", x, y);
	}

	void CreateBishop(int x, int y)
	{
		GameObject bishopObj = Instantiate(GameManager.Instance.bishopPrefab) as GameObject;
		Bishop bishop = bishopObj.GetComponent<Bishop>();
		SetupPiece(bishop, "Bishop", x, y);
	}

	void CreateKnight(int x, int y)
	{
		GameObject knightObj = Instantiate(GameManager.Instance.knightPrefab) as GameObject;
		Knight knight = knightObj.GetComponent<Knight>();
		SetupPiece(knight, "Knight", x, y);
	}

	void CreatePawn(int x, int y) 
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

	//////////////////////////////////////////////////////////
	// UPDATE
	//////////////////////////////////////////////////////////

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			CheckRayMouse();
		}
#if UNITY_IPHONE
		else if (Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Touch touch = Input.GetTouch(0);
			CheckRayTap(touch);
		}
#endif

		if (currentSelectedPiece)
		{
			IntVector2[] possibleMoves = currentSelectedPiece.GetPossibleMoves();
			for (int i = 0; i < possibleMoves.Length; i++)
			{
				IntVector2 move = possibleMoves[i];
				if (IsWithinBounds(move))
				{
					tileObjects[move.x, move.y].GetComponent<Tile>().ShowMove();

					if (pieces[move.x, move.y] != null)
					{
						pieces[move.x, move.y].potentialPush = true;
					}
				}
			}
		}
		else
		{
			ResetPossibleMoves();
		}
	}

	void CheckRayMouse()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleRayHit(hit);
		}
		else
		{
			if (currentSelectedPiece)
			{
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
			}
		}
	}

	void CheckRayTap(Touch touch)
	{
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleRayHit(hit);
		}
		else
		{
			if (currentSelectedPiece)
			{
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
			}
		}
	}

	void HandleRayHit(RaycastHit hit)
	{
		if (hit.collider.GetComponent<Piece>())
		{
			Piece piece = hit.collider.GetComponent<Piece>();

			if (piece == currentSelectedPiece)
			{
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
			}
			else if (piece != currentSelectedPiece)
			{
				if (!piece.potentialPush)
				{
					ResetPossibleMoves();

					currentSelectedPiece = piece;

					GameManager.Instance.SelectObject(currentSelectedPiece.transform);
				}
				else
				{
					// Force a move. We're probably going to be pushing too.
					IntVector2 coords = piece.GetCoordinates();
					Tile tile = tileObjects[coords.x, coords.y].GetComponent<Tile>();

					currentSelectedPiece.MoveToTile(tile);

					GameManager.Instance.Deselect();
					currentSelectedPiece = null;
				}
			}
		}
		else if (hit.collider.GetComponent<Tile>())
		{
			Tile tile = hit.collider.GetComponent<Tile>();

			if (currentSelectedPiece && tile.IsShowingMove())
			{
				// We just made a move!!! omg !!!!
				currentSelectedPiece.MoveToTile(tile);
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
			}
			else if (currentSelectedPiece && !tile.IsShowingMove())
			{
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
			}
		}
	}

	void ResetPossibleMoves()
	{
		for (int i = 0; i < tileObjects.GetLength(0); i++)
		{
			for (int j = 0; j < tileObjects.GetLength(0); j++)
			{
				tileObjects[i, j].GetComponent<Tile>().HideMove();

				if (pieces[i, j] != null)
				{
					pieces[i, j].potentialPush = false;
				}
			}
		}
	}

	//////////////////////////////////////////////////////////
	// DEACTIVATION
	//////////////////////////////////////////////////////////

	public override void Unload()
	{
		Destroy(gameObject);
	}
}
