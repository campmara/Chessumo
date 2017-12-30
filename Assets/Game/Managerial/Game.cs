using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Game : MonoBehaviour
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

	private GameObject topUIBarObj;
	private GameObject scoresButtonObj;
	private GameObject menuButtonObj;
	private GameObject resetButtonObj;

	private GameObject[,] tileObjects;
	private Piece[,] pieces;
	private PieceType nextRandomPieceType;
	private int numPiecesToSpawn = 0;

	private int scoreCombo;

	private GameObject pieceViewerObj;
	private NextPieceViewer pieceViewer;

	private IntVector2 nextRandomCoords;

	private bool moveInProgress = false;

	[ReadOnly, SerializeField] private Piece currentSelectedPiece;
	private List<IntVector2> currentPossibleMoves;
	private Piece currentMovePiece = null;
	private Tile currentMoveTile = null;

	/////////////////////////////////////////////////////////////////////
	// CODE
	/////////////////////////////////////////////////////////////////////

    private void Awake()
	{
		tileObjects = new GameObject[Constants.I.GridSize.x, Constants.I.GridSize.y];
		pieces = new Piece[Constants.I.GridSize.x, Constants.I.GridSize.y];

		currentSelectedPiece = null;
		currentPossibleMoves = new List<IntVector2>();
	}

	public void UpdatePieceCoordinates(Piece piece, IntVector2 oldCoordinates, IntVector2 newCoordinates)
	{
		pieces[oldCoordinates.x, oldCoordinates.y] = null;
		pieces[newCoordinates.x, newCoordinates.y] = piece;
	}

	public bool IsWithinBounds(IntVector2 coordinates)
	{
		return (coordinates.x >= 0 && coordinates.x < Constants.I.GridSize.x) && (coordinates.y >= 0 && coordinates.y < Constants.I.GridSize.y);
	}

	public bool CoordsOccupied(IntVector2 coordinates)
	{
		return pieces[coordinates.x, coordinates.y] != null;
	}

	public bool CurrentIsKnight()
	{
		return currentSelectedPiece.GetType() == typeof(Knight);
	}

	public void Load() 
	{
		StartCoroutine(SetupBoard());
	}

	IEnumerator SetupBoard()
	{
		AudioManager.Instance.PlayStartRelease();
		SetupPlayfield();

		yield return new WaitForSeconds(1.5f);

		AudioManager.Instance.PlayStartPiecesSpawn();
		PlacePieces();

		yield return new WaitForSeconds(1f);

		SetupPieceViewer();
	}

	/*
	IEnumerator SetupSavedBoard(string saveString)
	{
		SetupPlayfield();

		yield return new WaitForSeconds(1.5f);

		// parse the string and spawn old pieces
		while (!saveString.StartsWith("PV"))
		{
			string pieceKey = saveString.Remove(0, 2);
			bool isDisabled = saveString.Remove(0, 1) == "1" ? true : false;
			int xCoord = int.Parse(saveString.Remove(0, 1));
			int yCoord = int.Parse(saveString.Remove(0, 1));

			switch(pieceKey)
			{
				case "BI":
					Bishop bishop = CreateBishop(xCoord, yCoord);
					bishop.SetMoveDisabled(isDisabled);
					break;
				case "KI":
					King king = CreateKing(xCoord, yCoord);
					king.SetMoveDisabled(isDisabled);
					break;
				case "KN":
					Knight knight = CreateKnight(xCoord, yCoord);
					knight.SetMoveDisabled(isDisabled);
					break;
				case "PN":
					Pawn pawn = CreatePawn(xCoord, yCoord);
					pawn.SetMoveDisabled(isDisabled);
					break;
				case "QN":
					Queen queen = CreateQueen(xCoord, yCoord);
					queen.SetMoveDisabled(isDisabled);
					break;
				case "RK":
					Rook rook = CreateRook(xCoord, yCoord);
					rook.SetMoveDisabled(isDisabled);
					break;
			}
		}

		// parse piece viewer
		saveString.Remove(0, 2);
		string pieceViewerID = saveString.Remove(0, 2);
		int savedX = int.Parse(saveString.Remove(0, 1));
		SetupSavedPieceViewer(pieceViewerID, savedX);

		// parse score
		saveString.Remove(0, 2);
		int savedScore = int.Parse(saveString);
		GameManager.Instance.score.SetSavedScore(savedScore);
	}
	*/

	void SetupPlayfield()
	{
		for (int x = 0; x < Constants.I.GridSize.x; x++)
		{
			for (int y = 0; y < Constants.I.GridSize.y; y++)
			{
				GameObject tileObj = Instantiate(GameManager.Instance.tilePrefab) as GameObject;
				Tile tile = tileObj.GetComponent<Tile>();
				tile.gameObject.name = "Tile (" + x + ", " + y + ")";
				tile.transform.parent = transform;

				tile.transform.position = GameManager.Instance.CoordinateToPosition(new IntVector2(x, y));

				tile.SetInfo(x, y);
				
				// setup the colliders around the edges
				if (x == 0) tile.StretchCollider(-0.5f, 0f, 1f, 0f);
				if (x == Constants.I.GridSize.x - 1) tile.StretchCollider(0.5f, 0f, 1f, 0f);
				if (y == 0) tile.StretchCollider(0f, -0.5f, 0f, 1f);
				if (y == Constants.I.GridSize.y - 1) tile.StretchCollider(0f, 0.5f, 0f, 1f);

				tile.transform.localScale = new Vector3(0f, 0f, 1f);

				tileObjects[x, y] = tileObj;
			}
		}

		StartCoroutine(TileGrowRoutine());
	}

	void SetupPieceViewer()
	{
		pieceViewerObj = Instantiate(GameManager.Instance.nextPieceViewerPrefab) as GameObject;
		pieceViewerObj.name = "Piece Viewer";
		pieceViewerObj.transform.parent = transform;
		//pieceViewerObj.transform.position = new Vector3(0f, Constants.I.ScoreRaisedY - 1f, 0f);
		pieceViewer = pieceViewerObj.GetComponent<NextPieceViewer>();

		DecideNextRandomPiece();
	}

	private IEnumerator TileGrowRoutine()
	{
		int[] order = new int[tileObjects.GetLength(0) * tileObjects.GetLength(1)];
		for (int i = 0; i < order.Length; i++)
		{
			order[i] = i;
		}
		// now shuffle
		for (int t = 0; t < order.Length; t++ )
        {
            int tmp = order[t];
            int r = Random.Range(t, order.Length);
            order[t] = order[r];
            order[r] = tmp;
        }

		for (int i = 0; i < order.Length; i++)
		{
			int x = order[i] % tileObjects.GetLength(0);
			int y = order[i] / tileObjects.GetLength(1);

			Tile tile = tileObjects[x, y].GetComponent<Tile>();

			yield return new WaitForSeconds(Random.Range(0f, 0.1f));

			tile.transform.DOScale(new Vector3(1f, 1f, 1f), 1f)
				.SetEase(Ease.OutBack);
		}
	}

#if UNITY_EDITOR

	//////////////////////////////////////////////////////////
	// EDITOR UPDATE
	//////////////////////////////////////////////////////////

	void Update()
	{
		if (moveInProgress)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			OnFingerDown();
		}

		if (currentSelectedPiece)
		{
			OnFingerMove();
		}

		if (Input.GetMouseButtonUp(0))
		{
			OnFingerUp();
		}
	}

	//////////////////////////////////////////////////////////
	// INPUT CALLBACKS
	//////////////////////////////////////////////////////////

	void OnFingerDown()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleDownRayHit(hit);
		}
	}

	void OnFingerMove()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleMoveRayHit(hit);
		}
	}

	void OnFingerUp()
	{
		// Try and initiate the move.
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleUpRayHit(hit);
		}
		else
		{
			ResetPossibleMoves();
		}
	}

#elif UNITY_IPHONE

	//////////////////////////////////////////////////////////
	// IPHONE UPDATE
	//////////////////////////////////////////////////////////

	void Update()
	{
		if (moveInProgress)
		{
			return;
		}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Touch touch = Input.GetTouch(0);
			OnFingerDown(touch);
		}

		if (currentSelectedPiece && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
		{
			Touch touch = Input.GetTouch(0);
			OnFingerMove(touch);
		}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
		{
			Touch touch = Input.GetTouch(0);
			OnFingerUp(touch);
		}
	}

	//////////////////////////////////////////////////////////
	// INPUT CALLBACKS
	//////////////////////////////////////////////////////////

	void OnFingerDown(Touch touch)
	{
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleDownRayHit(hit);
		}
	}

	void OnFingerMove(Touch touch)
	{
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleMoveRayHit(hit);
		}
	}

	void OnFingerUp(Touch touch)
	{
		Ray ray = Camera.main.ScreenPointToRay(touch.position);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, Mathf.Infinity))
		{
			HandleUpRayHit(hit);
		}
		else
		{
			ResetPossibleMoves();
			EndDrawingMove();
		}
	}

#endif

	void HandleDownRayHit(RaycastHit hit)
	{
		if (GameManager.Instance.settingsMenu.IsOpen()) return;

		if (hit.collider.GetComponent<Piece>())
		{
			Piece piece = hit.collider.GetComponent<Piece>();

			if (!piece.potentialPush && !piece.GetMoveDisabled())
			{	
				//ResetPossibleMoves();
				SelectPiece(piece);
			}
		}
	}

	private IntVector2 lastMoveCoords;

	void HandleMoveRayHit(RaycastHit hit)
	{
		if (GameManager.Instance.settingsMenu.IsOpen()) return;

		if (hit.collider.GetComponent<Piece>() == currentSelectedPiece)
		{
			//MovePathManager.Instance.BeginPath(currentSelectedPiece.GetCoordinates());
			//currentMoveTile = null;
			//currentMovePiece = null;
			DrawReturnedToStartPiece();
			return;
		}

		Piece piece = hit.collider.GetComponent<Piece>();
		Tile tile = hit.collider.GetComponent<Tile>();

		// Tells the arrow system to update.
		// CHECK TO MAKE SURE THAT WHAT YOU HIT IS WITHIN POSSIBLE MOVESPACE OF THE CURRENT PIECE. THIS IS IMPORTANT.
		if (tile != null)
		{
			// Check to make sure we're processing a new tile and not a repeat one.
			if (tile == currentMoveTile)
			{
				return;
			}

			if (!CheckCoordsWithinPossibleMoves(tile.GetCoordinates()))
			{
				return;
			}

			
			if (CurrentIsKnight())
			{
				Move move = currentSelectedPiece.Moveset.DetermineKnightMove(tile.GetCoordinates(), tile.GetCoordinates() - lastMoveCoords);
				if (currentSelectedPiece.GetComponent<Knight>().IsValidMove(move))
				{
					DrawMove(move);
				}
			}
			else
			{
				Move move = currentSelectedPiece.Moveset.DetermineMove(tile.GetCoordinates());
				DrawMove(move);
			}

			currentMoveTile = tile;
			lastMoveCoords = tile.GetCoordinates();
			currentMovePiece = null;
		}
		else if (piece != null)
		{
			// Check to make sure we're processing a new tile and not a repeat one.
			if (piece == currentMovePiece)
			{
				return;
			}

			if (!CheckCoordsWithinPossibleMoves(piece.GetCoordinates()))
			{
				return;
			}

			if (CurrentIsKnight()) // Check to make sure this is a valid movement.
			{
				Move move = currentSelectedPiece.Moveset.DetermineKnightMove(piece.GetCoordinates(), piece.GetCoordinates() - lastMoveCoords);
				if (currentSelectedPiece.GetComponent<Knight>().IsValidMove(move))
				{
					DrawMove(move);
				}
			}
			else
			{
				Move move = currentSelectedPiece.Moveset.DetermineMove(piece.GetCoordinates());
				DrawMove(move);
			}

			currentMovePiece = piece;
			lastMoveCoords = piece.GetCoordinates();
			currentMoveTile = tileObjects[piece.GetCoordinates().x, piece.GetCoordinates().y].GetComponent<Tile>();
		}
	}

	void HandleUpRayHit(RaycastHit hit)
	{
		if (GameManager.Instance.settingsMenu.IsOpen()) return;

		if (hit.collider.GetComponent<Piece>()) // up on a piece
		{
			Piece piece = hit.collider.GetComponent<Piece>();

			if (piece.potentialPush && MoveIsOnPath(piece.GetCoordinates()))
			{
				if (CurrentIsKnight())
				{
					IntVector2 dir = tilesToRaise[0] - currentSelectedPiece.GetCoordinates();
					currentSelectedPiece.GetComponent<Knight>().SetInitialDirection(dir);
				}

				currentSelectedPiece.MoveTo(piece.GetCoordinates(), false, 0, 0);
				OnMoveInitiated();
			}
			else
			{
				AudioManager.Instance.PlayPiecePickup();
				ResetPossibleMoves();
			}	
		}
		else if (hit.collider.GetComponent<Tile>()) // up on a tile
		{
			Tile tile = hit.collider.GetComponent<Tile>();
			
			if (currentSelectedPiece && tile.GetCoordinates() == currentSelectedPiece.GetCoordinates())
			{
				AudioManager.Instance.PlayPiecePickup();
				ResetPossibleMoves();
			}
			else if (currentSelectedPiece && tile.GetState() == TileState.DRAWN)
			{
				if (CurrentIsKnight())
				{
					IntVector2 dir = tilesToRaise[0] - currentSelectedPiece.GetCoordinates();
					currentSelectedPiece.GetComponent<Knight>().SetInitialDirection(dir);
				}

				// We just made a move!!! omg !!!!
				currentSelectedPiece.MoveTo(tile.GetCoordinates(), false, 0, 0);
				OnMoveInitiated();
			}
			else if (currentSelectedPiece && tile.GetState() != TileState.DRAWN)
			{
				AudioManager.Instance.PlayPiecePickup();
				ResetPossibleMoves();
			}
		}
	}

	private bool MoveIsOnPath(IntVector2 coords)
	{
		for (int i = 0; i < tilesToRaise.Length; i++)
		{
			if (tilesToRaise[i] == coords) return true;
		}
		
		return false;
	}

	private void BeginDrawingMove()
	{
		tilesToRaise = null;
		MovePathManager.Instance.BeginPath(currentSelectedPiece.GetCoordinates());
	}

	private IntVector2[] tilesToRaise;

	private void DrawMove(Move move)
	{
		ClearPreviousDraw();

		// Handle new path.
		if (CurrentIsKnight())
		{
			currentSelectedPiece.GetComponent<Knight>().UpdateKnightMove(move);

			IntVector2[] temp = MovePathManager.Instance.CalculateKnightPath(tilesToRaise, move);
			tilesToRaise = temp;
		}
		else
		{
			tilesToRaise = MovePathManager.Instance.CalculatePath(move);
		}

		if (tilesToRaise == null) return;

		for (int i = 0; i < tilesToRaise.Length; i++)
		{
			Tile tile = tileObjects[tilesToRaise[i].x, tilesToRaise[i].y].GetComponent<Tile>();
			if (tile.GetState() == TileState.KNIGHT_TRAVERSABLE)
			{
				tile.SetState(TileState.KNIGHT_TRAVERSED, currentSelectedPiece.FullColor);
			}
			else
			{
				tile.SetState(TileState.DRAWN, currentSelectedPiece.FullColor);
				if (pieces[tilesToRaise[i].x, tilesToRaise[i].y] != null) pieces[tilesToRaise[i].x, tilesToRaise[i].y].PickPieceUp();
			}
		}
	}

	private void ClearPreviousDraw()
	{
		if (tilesToRaise != null)
		{
			for (int i = 0; i < tilesToRaise.Length; i++)
			{
				Tile tile = tileObjects[tilesToRaise[i].x, tilesToRaise[i].y].GetComponent<Tile>();
				if(tile.GetState() == TileState.KNIGHT_TRAVERSED)
				{
					tile.SetState(TileState.KNIGHT_TRAVERSABLE, Color.black);
				}
				else
				{
					tile.SetState(TileState.POSSIBLE, currentSelectedPiece.SubduedColor);
				}
				if (pieces[tilesToRaise[i].x, tilesToRaise[i].y] != null) pieces[tilesToRaise[i].x, tilesToRaise[i].y].SetPieceDown();
			}
		}
	}

	private void DrawReturnedToStartPiece()
	{
		if (tilesToRaise != null)
		{
			for (int i = 0; i < tilesToRaise.Length; i++)
			{
				Tile tile = tileObjects[tilesToRaise[i].x, tilesToRaise[i].y].GetComponent<Tile>();
				if(tile.GetState() == TileState.KNIGHT_TRAVERSED)
				{
					tile.SetState(TileState.KNIGHT_TRAVERSABLE, Color.black);
				}
				else
				{
					tile.SetState(TileState.POSSIBLE, currentSelectedPiece.SubduedColor);
				}
				if (pieces[tilesToRaise[i].x, tilesToRaise[i].y] != null) pieces[tilesToRaise[i].x, tilesToRaise[i].y].SetPieceDown();
			}
		}

		tilesToRaise = null;
		lastMoveCoords = currentSelectedPiece.GetCoordinates();
		if (CurrentIsKnight())
		{
			currentSelectedPiece.GetComponent<Knight>().ResetKnight();
			currentMovePiece = null;
			currentMoveTile = null;
		}
	}

	private void EndDrawingMove()
	{
		for (int i = 0; i < tileObjects.GetLength(0); i++)
		{
			for (int j = 0; j < tileObjects.GetLength(1); j++)
			{
				tileObjects[i, j].GetComponent<Tile>().SetState(TileState.DEFAULT, Color.black);

				if (pieces[i, j] != null)
				{
					pieces[i, j].SetPieceDown();
					pieces[i, j].SetPushPotential(false);
				}
			}
		}

		MovePathManager.Instance.EndPath();
	}

	private bool CheckCoordsWithinPossibleMoves(IntVector2 coords)
	{
		if (CurrentIsKnight())
		{
			IntVector2[] secondaryMoves = currentSelectedPiece.GetComponent<Knight>().GetSecondaryMoves();

			for (int i = 0; i < currentPossibleMoves.Count; i++)
			{
				if (currentPossibleMoves[i] == coords)
				{
					return true;
				}
			}

			for (int i = 0; i < secondaryMoves.Length; i++)
			{
				if (secondaryMoves[i] == coords)
				{
					return true;
				}
			}
		}
		else
		{
			for (int i = 0; i < currentPossibleMoves.Count; i++)
			{
				if (currentPossibleMoves[i] == coords)
				{
					return true;
				}
			}
		}

		return false;
	}

    private float waitTime = 0f;
    public float GetWaitTime()
    {
        return waitTime;
    }
    public void IncrementWaitTime()
    {
        waitTime += Constants.I.PieceMoveTime;
    }
    public void ResetWaitTimeOnChangeDirection() // for use specifically with MoveXThenY functions
    {
        waitTime = 0f;
    }

	public void OnPieceMove(Piece piece, IntVector2 direction, int distance)
	{
		IntVector2 oldCoordinates = piece.GetCoordinates();
		IntVector2 currentCheckingCoords = oldCoordinates;
		int absDist = Mathf.Abs(distance);
		int distFromPushingPiece = 0;
        int numPushedPieces = 0;

		for (int i = 0; i < absDist; i++)
		{
			currentCheckingCoords += direction;
			distFromPushingPiece++;

			if (pieces[currentCheckingCoords.x, currentCheckingCoords.y] != null)
			{
				int pushDistance = (absDist - distFromPushingPiece) + 1;
				IntVector2 push = currentCheckingCoords + direction * pushDistance;
				numPushedPieces++;

				if (IsWithinBounds(push))
				{
					// Push in the grid.
					pieces[currentCheckingCoords.x, currentCheckingCoords.y].MoveTo(push, true, distFromPushingPiece, numPushedPieces);
                    break;
				}
				else
				{
					// PUSH OFF THE GRID.
					PieceOffGrid(pieces[currentCheckingCoords.x, currentCheckingCoords.y], push, absDist, distFromPushingPiece, numPushedPieces);
					pieces[currentCheckingCoords.x, currentCheckingCoords.y] = null;
				}
			}
            else
            {
                IncrementWaitTime();
            }
		}
	}

	private void PieceOffGrid(Piece piece, IntVector2 pushCoordinates, int travelDist, int distFromPushingPiece, int numPushedPieces)
	{
		numPiecesToSpawn++;

		Vector2 offGridPosition = GameManager.Instance.CoordinateToPosition(pushCoordinates);
		
		StartCoroutine(MovePieceOffGrid(piece, offGridPosition, travelDist, distFromPushingPiece, numPushedPieces));
	}

	protected IEnumerator MovePieceOffGrid(Piece piece, Vector2 position, int travelDist, int distFromPushingPiece, int numPushedPieces)
	{
        //float waitTime = piece.CalculatePushWaitTime(distFromPushingPiece, numPushedPieces);
        //yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(GetWaitTime());

		AudioManager.Instance.PlayPieceOffGrid();

		GameObject pieceObj = piece.gameObject;

		Vector3 newPos = new Vector3(position.x, position.y, pieceObj.transform.position.z);

		float duration = Constants.I.PieceMoveTime * (travelDist - Mathf.Clamp(distFromPushingPiece - 1, 0, distFromPushingPiece + 1));
		Tween tween = pieceObj.transform.DOMove(newPos, duration)
			.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		piece.SetSortingLayer("Falling Pieces");
		Destroy(piece); // Destroy the piece class so it won't get touched.

		tween = pieceObj.transform.DOMoveY(-6f, 0.75f)
			.SetEase(Ease.InCubic);
		yield return tween.WaitForCompletion();

		// Score a point and handle the score combo.
        HandleScorePoint(piece);

		Destroy(pieceObj);
	}

    void HandleScorePoint(Piece piece)
	{
		if (!Constants.I.CombosEnabled)
		{
			return;
		}

		Debug.Log("Score Combo Incremented, Current Number: " + scoreCombo);
		scoreCombo++;

		if (scoreCombo >= Constants.I.GridSize.x - 1)
		{
            GameManager.Instance.scoreEffect.OnThreeScored(piece.FullColor);

			for (int i = 0; i < Constants.I.ScoreThreeAmount; i++)
			{
				GameManager.Instance.score.ScorePoint();
			}
		}
        else if (scoreCombo >= 2)
        {
            GameManager.Instance.scoreEffect.OnTwoScored(piece.FullColor);
			
			for (int i = 0; i < Constants.I.ScoreTwoAmount; i++)
			{
				GameManager.Instance.score.ScorePoint();
			}
        }
        else if (scoreCombo >= 1)
        {
            GameManager.Instance.scoreEffect.OnOneScored(piece.FullColor);
			
			for (int i = 0; i < Constants.I.ScoreOneAmount; i++)
			{
				GameManager.Instance.score.ScorePoint();
			}
        }
	}

	void SelectPiece(Piece piece)
	{
		// Play Pickup Sound
		AudioManager.Instance.PlayPiecePickup();

		// Handle Pickup Effect
		piece.PickPieceUp();
		currentMoveTile = tileObjects[piece.GetCoordinates().x, piece.GetCoordinates().y].GetComponent<Tile>();
		currentMoveTile.SetState(TileState.DRAWN, piece.FullColor);

		currentSelectedPiece = piece;

		lastMoveCoords = piece.GetCoordinates();

		// Begin drawing the path.
		BeginDrawingMove();

		currentSelectedPiece.DetermineMoveset();
		currentPossibleMoves = currentSelectedPiece.GetPossibleMoves();
		for (int i = 0; i < currentPossibleMoves.Count; i++)
		{
			IntVector2 move = currentPossibleMoves[i];
			if (IsWithinBounds(move))
			{
				tileObjects[move.x, move.y].GetComponent<Tile>().SetState(TileState.POSSIBLE, currentSelectedPiece.SubduedColor);

				if (pieces[move.x, move.y] != null)
				{
					//pieces[move.x, move.y].PickPieceUp();
					pieces[move.x, move.y].SetPushPotential(true);
				}
			}
		}

		// Handle Knight selection. We gotta get its secondary moves and highlight those too.
		if (CurrentIsKnight())
		{
			IntVector2[] secondaryMoves = currentSelectedPiece.GetComponent<Knight>().GetSecondaryMoves();

			for (int i = 0; i < secondaryMoves.Length; i++)
			{
				IntVector2 move = secondaryMoves[i];
				if (IsWithinBounds(move))
				{
					tileObjects[move.x, move.y].GetComponent<Tile>().SetState(TileState.KNIGHT_TRAVERSABLE, Color.black);
				}
			}
		}
	}

	public void OnMoveInitiated()
	{
		moveInProgress = true;

		AudioManager.Instance.PlayMoveHit();

		Debug.Log("Score Combo Zeroed");
		scoreCombo = 0;
	}

	public void OnMoveEnded()
	{
		currentSelectedPiece.SetMoveDisabled(true);
		ResetPossibleMoves();

        waitTime = 0f;

		// Spawn the needed amount of pieces.
		for (int i = 0; i < numPiecesToSpawn; i++)
		{
			PlaceNextRandomPiece();
		}

		numPiecesToSpawn = 0;

		// Checks whether it should end the game or enable NPV movement.
		CheckPieceCount();

		moveInProgress = false;
	}

	void CheckPieceCount()
	{
		int movablePieceCount = 0;

		for (int i = 0; i < pieces.GetLength(0); i++)
		{
			for (int j = 0; j < pieces.GetLength(1); j++)
			{
				if (pieces[i, j] != null && !pieces[i, j].GetMoveDisabled()) 
				{
					movablePieceCount++;
				}
			}
		}

		if (movablePieceCount <= 0)
		{
			StartCoroutine(GameEndRoutine());
		}
	}

	IEnumerator GameEndRoutine()
	{
		AudioManager.Instance.PlayGameOver();
		StartCoroutine(DropPieces());
		StartCoroutine(DropTiles());

		yield return new WaitForSeconds(1.44f);

		AudioManager.Instance.PlayNPVFade();
		pieceViewer.FadeOut();

		yield return new WaitForSeconds(3f);

		GameManager.Instance.score.SubmitScore();
		GameManager.Instance.highScore.PullHighScore();
		GameManager.Instance.OnGameEnd();
	}

	IEnumerator DropPieces()
	{
		for (int i = 0; i < pieces.GetLength(0); i++)
		{
			for (int j = 0; j < pieces.GetLength(1); j++)
			{
				if (pieces[i, j] != null) 
				{
					float duration = Random.Range(0.4f, 1.5f);

					tileObjects[i, j].transform.DOMoveY(-10f, duration).SetEase(Ease.InQuint);
					pieces[i, j].transform.DOMoveY(-10f, duration).SetEase(Ease.InQuint);

					yield return new WaitForSeconds(0.01f);
				}
			}
		}
	}

	IEnumerator DropTiles()
	{
		for (int i = 0; i < tileObjects.GetLength(0); i++)
		{
			for (int j = 0; j < tileObjects.GetLength(1); j++)
			{
				if (pieces[i, j] == null)
				{
					// If there are no pieces on this then we can drop.
					tileObjects[i, j].transform.DOMoveY(-10f, Random.Range(0.4f, 1.5f)).SetEase(Ease.InQuint);
					yield return new WaitForSeconds(0.01f);
				}
			}
		}
	}

	public void Unload() 
	{
		Destroy(gameObject);
	}

	public void ResetPossibleMoves()
	{
		EndDrawingMove();

		if (currentSelectedPiece != null)
		{
			if (CurrentIsKnight())
			{
				currentSelectedPiece.GetComponent<Knight>().ResetKnight();
			}

			currentSelectedPiece.SetPieceDown();
			currentSelectedPiece.SetPushPotential(false);
			currentSelectedPiece = null;
		}
		
		currentPossibleMoves = null;
		currentMovePiece = null;
		currentMoveTile = null;
	}

	List<IntVector2> tempListUnoccupied = new List<IntVector2>();
	IntVector2 RandomCoordinates()
	{
		tempListUnoccupied.Clear();
		for (int i = 0; i < tileObjects.GetLength(0); i++)
		{
			for (int j = 0; j < tileObjects.GetLength(1); j++)
			{
				IntVector2 tileCoords = tileObjects[i, j].GetComponent<Tile>().GetCoordinates();
				if (pieces[tileCoords.x, tileCoords.y] == null)
				{
					tempListUnoccupied.Add(tileCoords);
				}
			}
		}

		return tempListUnoccupied[Random.Range(0, tempListUnoccupied.Count)];

		//return new IntVector2(Random.Range(0, Constants.I.GridSize.x), Random.Range(0, Constants.I.GridSize.y));
	}

	IntVector2 RandomCoordinatesInColumn(int x)
	{
		int count = 0;
		for (int i = 0; i < tileObjects.GetLength(1); i++)
		{
			if (pieces[x, i] != null)
			{
				count++;
			}
		}

		if (count >= Constants.I.GridSize.y - 1)
		{
			return RandomCoordinates();
		}
		else
		{
			return new IntVector2(x, Random.Range(0, Constants.I.GridSize.y));
		}
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

	private void DecideNextRandomPiece()
	{
		int rand = Random.Range(0, 6);

		switch (rand)
		{
			case 0:
				nextRandomPieceType = PieceType.KING;
				pieceViewer.ShowKing();
				break;
			case 1:
				nextRandomPieceType = PieceType.QUEEN;
				pieceViewer.ShowQueen();
				break;
			case 2:
				nextRandomPieceType = PieceType.ROOK;
				pieceViewer.ShowRook();
				break;
			case 3:
				nextRandomPieceType = PieceType.BISHOP;
				pieceViewer.ShowBishop();
				break;
			case 4:
				nextRandomPieceType = PieceType.KNIGHT;
				pieceViewer.ShowKnight();
				break;
			case 5:
				nextRandomPieceType = PieceType.PAWN;
				pieceViewer.ShowPawn();
				break;
			default:
				break;
		}

		nextRandomCoords = RandomCoordinates();

		// Position the piece viewer at the top above the column it needs to be at.
		pieceViewer.PositionAlongTop(nextRandomCoords);
	}

	protected void PlaceNextRandomPiece()
	{
		while (pieces[nextRandomCoords.x, nextRandomCoords.y] != null)
		{
			nextRandomCoords = RandomCoordinatesInColumn(nextRandomCoords.x);
		}

		switch (nextRandomPieceType)
		{
			case PieceType.KING:
				CreateKing(nextRandomCoords.x, nextRandomCoords.y);
				break;
			case PieceType.QUEEN:
				CreateQueen(nextRandomCoords.x, nextRandomCoords.y);
				break;
			case PieceType.ROOK:
				CreateRook(nextRandomCoords.x, nextRandomCoords.y);
				break;
			case PieceType.BISHOP:
				CreateBishop(nextRandomCoords.x, nextRandomCoords.y);
				break;
			case PieceType.KNIGHT:
				CreateKnight(nextRandomCoords.x, nextRandomCoords.y);
				break;
			case PieceType.PAWN:
				CreatePawn(nextRandomCoords.x, nextRandomCoords.y);
				break;
			default:
				break;
		}

		DecideNextRandomPiece();
	}

	private void PlacePieces()
	{
		StartCoroutine(PlacePiecesRoutine());
	}

	private IEnumerator PlacePiecesRoutine()
	{
		float[] waitTimes = new float[Constants.I.StartingPieceCount];
		waitTimes[0] = 0.1f;
		waitTimes[1] = 0.1f;
		waitTimes[2] = 0.1f;
		waitTimes[3] = 0.2f;
		waitTimes[4] = 0f;

		// Spawn all the starting pieces.
		for (int i = 0; i < Constants.I.StartingPieceCount; i++)
		{
			PlaceRandomPiece();
			yield return new WaitForSeconds(waitTimes[i]);
		}
	}

	protected King CreateKing(int x, int y)
	{
		GameObject kingObj = Instantiate(GameManager.Instance.kingPrefab) as GameObject;
		King king = kingObj.GetComponent<King>();
		SetupPiece(king, "King", x, y);

		return king;
	}

	protected Queen CreateQueen(int x, int y)
	{
		GameObject queenObj = Instantiate(GameManager.Instance.queenPrefab) as GameObject;
		Queen queen = queenObj.GetComponent<Queen>();
		SetupPiece(queen, "Queen", x, y);

		return queen;
	}

	protected Rook CreateRook(int x, int y)
	{
		GameObject rookObj = Instantiate(GameManager.Instance.rookPrefab) as GameObject;
		Rook rook = rookObj.GetComponent<Rook>();
		SetupPiece(rook, "Rook", x, y);

		return rook;
	}

	protected Bishop CreateBishop(int x, int y)
	{
		GameObject bishopObj = Instantiate(GameManager.Instance.bishopPrefab) as GameObject;
		Bishop bishop = bishopObj.GetComponent<Bishop>();
		SetupPiece(bishop, "Bishop", x, y);

		return bishop;
	}

	protected Knight CreateKnight(int x, int y)
	{
		GameObject knightObj = Instantiate(GameManager.Instance.knightPrefab) as GameObject;
		Knight knight = knightObj.GetComponent<Knight>();
		SetupPiece(knight, "Knight", x, y);

		return knight;
	}

	protected Pawn CreatePawn(int x, int y) 
	{
		GameObject pawnObj = Instantiate(GameManager.Instance.pawnPrefab) as GameObject;
		Pawn pawn = pawnObj.GetComponent<Pawn>();
		SetupPiece(pawn, "Pawn", x, y);

		return pawn;
	}

	void SetupPiece(Piece piece, string name, int x, int y)
	{
		piece.gameObject.name = name;
		piece.transform.parent = transform;
		piece.transform.position = new Vector3(tileObjects[x, y].transform.position.x, tileObjects[x, y].transform.position.y, piece.transform.position.z);
		piece.SetInfo(x, y, this);

		pieces[x, y] = piece;
	}

	/*
	// if there's a piece, itll be "PN0" or "PN1"
	// if there is no piece, it'll be "NP" to signify no piece
	public string GenerateSaveString()
	{
		string saveString = "";

		for (int i = 0; i < pieces.GetLength(0); i++)
		{
			for (int j = 0; j < pieces.GetLength(1); j++)
			{
				if (pieces[i, j] != null)
				{
					saveString += pieces[i, j].pieceID;
					saveString += pieces[i, j].GetMoveDisabled() == true ? "1" : "0";
					saveString += pieces[i, j].GetCoordinates().x.ToString();
					saveString += pieces[i, j].GetCoordinates().y.ToString();
				}
			}
		}

		saveString += "PV" + pieceViewer.nextPieceID.ToString() + pieceViewer.nextX.ToString();
		saveString += "SC" + GameManager.Instance.score.GetScore().ToString();

		Debug.Log(saveString);
		return saveString;
	}
	*/
}