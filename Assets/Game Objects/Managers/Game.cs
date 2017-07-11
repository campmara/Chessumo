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

	private GameObject[,] tileObjects;
	private Piece[,] pieces;
	private PieceType nextRandomPieceType;
	private int numPiecesToSpawn = 0;

	private GameObject scoreObj;
	private Score score;
	private GameObject scoreEffectObj;
	private ScoreEffect scoreEffect;

	private int scoreCombo;
	
	private GameObject highScoreObj;
	private HighScore highScore;

	private GameObject pieceViewerObj;
	private NextPieceViewer pieceViewer;

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

	public void Load() 
	{
		StartCoroutine(SetupBoard());

		SetupScore();
		SetupPieceViewer();
	}

	void SetupScore()
	{
		scoreObj = Instantiate(GameManager.Instance.scorePrefab) as GameObject;
		scoreObj.name = "Score";
		scoreObj.transform.parent = transform;
		scoreObj.transform.position = new Vector3(0f, Constants.I.ScoreRaisedY, 0f);
		score = scoreObj.GetComponent<Score>();
		score.Reset();

		scoreEffectObj = Instantiate(GameManager.Instance.scoreEffectPrefab) as GameObject;
		scoreEffectObj.name = "Score Effect";
		scoreEffectObj.transform.parent = transform;
		scoreEffect = scoreEffectObj.GetComponent<ScoreEffect>();

		highScoreObj = Instantiate(GameManager.Instance.highScorePrefab) as GameObject;
		highScoreObj.name = "High Score";
		highScoreObj.transform.parent = transform;
		highScoreObj.transform.position = new Vector3(0f, Constants.I.ScoreRaisedY, 0f);
		highScore = highScoreObj.GetComponent<HighScore>();
		highScore.PullHighScore();
	}

	void SetupPieceViewer()
	{
		pieceViewerObj = Instantiate(GameManager.Instance.nextPieceViewerPrefab) as GameObject;
		pieceViewerObj.name = "Piece Viewer";
		pieceViewerObj.transform.parent = transform;
		pieceViewerObj.transform.position = new Vector3(0f, Constants.I.ScoreRaisedY - 1f, 0f);
		pieceViewer = pieceViewerObj.GetComponent<NextPieceViewer>();

		DecideNextRandomPiece();
	}

	IEnumerator SetupBoard()
	{
		SetupPlayfield();

		yield return new WaitForSeconds(1.5f);

		PlacePieces();
	}

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

				tile.SetInfo(x, y, this);

				tileObjects[x, y] = tileObj;
			}
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
			MovePathManager.Instance.EndPath();
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
			OnFingerUp();
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
			MovePathManager.Instance.Reset();
		}
	}

#endif

	void HandleDownRayHit(RaycastHit hit)
	{
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

	void HandleMoveRayHit(RaycastHit hit)
	{
		if (hit.collider.GetComponent<Piece>() == currentSelectedPiece)
		{
			MovePathManager.Instance.BeginPath(currentSelectedPiece.GetCoordinates(), isReturn: true);
			currentMoveTile = null;
			currentMovePiece = null;
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

			Move move = currentSelectedPiece.Moveset.DetermineMove(tile.GetCoordinates());
			MovePathManager.Instance.DrawPath(move);

			currentMoveTile = tile;
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

			Move move = currentSelectedPiece.Moveset.DetermineMove(piece.GetCoordinates());
			MovePathManager.Instance.DrawPath(move);

			currentMovePiece = piece;
		}
	}

	void HandleUpRayHit(RaycastHit hit)
	{
		if (hit.collider.GetComponent<Piece>()) // up on a piece
		{
			Piece piece = hit.collider.GetComponent<Piece>();

			if (piece.potentialPush)
			{
				if (currentSelectedPiece.GetType() == typeof(Knight))
				{
					currentSelectedPiece.GetComponent<Knight>().SetInitialDirection(MovePathManager.Instance.GetFirstMoveDirection());
				}

				currentSelectedPiece.MoveTo(piece.GetCoordinates(), false, 0, 0);
				OnMoveInitiated();
			}
			else
			{
				ResetPossibleMoves();
				MovePathManager.Instance.EndPath();
			}	
		}
		else if (hit.collider.GetComponent<Tile>()) // up on a tile
		{
			Tile tile = hit.collider.GetComponent<Tile>();

			if (currentSelectedPiece && tile.IsShowingMove())
			{
				if (currentSelectedPiece.GetType() == typeof(Knight))
				{
					currentSelectedPiece.GetComponent<Knight>().SetInitialDirection(MovePathManager.Instance.GetFirstMoveDirection());
				}

				// We just made a move!!! omg !!!!
				currentSelectedPiece.MoveTo(tile.GetCoordinates(), false, 0, 0);
				OnMoveInitiated();
			}
			else if (currentSelectedPiece && !tile.IsShowingMove())
			{
				ResetPossibleMoves();
				MovePathManager.Instance.EndPath();
			}
		}
	}

	private bool CheckCoordsWithinPossibleMoves(IntVector2 coords)
	{
		if (currentSelectedPiece.GetType() == typeof(Knight))
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
					//Debug.Log("Pushing within bounds.");
					pieces[currentCheckingCoords.x, currentCheckingCoords.y].MoveTo(push, true, distFromPushingPiece, numPushedPieces);
				}
				else
				{
					// PUSH OFF THE GRID.
					//Debug.Log("YOU JUST PUSHED A PIECE OFF THE GRID");
					PieceOffGrid(pieces[currentCheckingCoords.x, currentCheckingCoords.y], push, absDist, distFromPushingPiece, numPushedPieces);
					pieces[currentCheckingCoords.x, currentCheckingCoords.y] = null;
				}
			}
			else
			{
				//Debug.Log("Normal Move. No Push");
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
		int clampPushed = Mathf.Clamp(numPushedPieces - 1, 0, numPushedPieces + 1);
		int clampDist = Mathf.Clamp((distFromPushingPiece - 1) - clampPushed, 0, distFromPushingPiece + 1);
		float waitTime = Constants.I.PieceMoveTime * clampDist;
		yield return new WaitForSeconds(waitTime);

		GameObject pieceObj = piece.gameObject;

		Vector3 newPos = new Vector3(position.x, position.y, pieceObj.transform.position.z);

		float duration = Constants.I.PieceMoveTime * (travelDist - Mathf.Clamp(distFromPushingPiece - 1, 0, distFromPushingPiece + 1));
		Tween tween = pieceObj.transform.DOMove(newPos, duration)
			.SetEase(Ease.Linear);
		yield return tween.WaitForCompletion();

		piece.HandleFallingSortingOrder();
		Destroy(piece); // Destroy the piece class so it won't get touched.

		tween = pieceObj.transform.DOMoveY(-6f, 0.75f)
			.SetEase(Ease.InCubic);
		yield return tween.WaitForCompletion();

		// Score a point and handle the score combo.
		score.ScorePoint();
		scoreEffect.OnPointScored();
		IncrementScoreCombo();

		Destroy(pieceObj);
	}

	void IncrementScoreCombo()
	{
		if (!Constants.I.CombosEnabled)
		{
			return;
		}

		Debug.Log("Score Combo Incremented, Current Number: " + scoreCombo);
		scoreCombo++;

		if (scoreCombo >= Constants.I.GridSize.x - 1)
		{
			// We've scored the combo!
			Debug.Log("Scored Combo.");

			for (int i = 0; i < scoreCombo; i++)
			{
				score.ScorePoint();
			}
		}
	}

	void SelectPiece(Piece piece)
	{
		// Handle Pickup Effect
		piece.PickPieceUp();
		tileObjects[piece.GetCoordinates().x, piece.GetCoordinates().y].GetComponent<Tile>().ShowMove();

		currentSelectedPiece = piece;

		// Begin drawing the path.
		MovePathManager.Instance.BeginPath(currentSelectedPiece.GetCoordinates(), isReturn: false);

		currentSelectedPiece.DetermineMoveset();
		currentPossibleMoves = currentSelectedPiece.GetPossibleMoves();
		for (int i = 0; i < currentPossibleMoves.Count; i++)
		{
			IntVector2 move = currentPossibleMoves[i];
			if (IsWithinBounds(move))
			{
				tileObjects[move.x, move.y].GetComponent<Tile>().ShowMove();

				if (pieces[move.x, move.y] != null)
				{
					pieces[move.x, move.y].PickPieceUp();
					pieces[move.x, move.y].SetPushPotential(true);
				}
			}
		}
	}

	public void OnMoveInitiated()
	{
		moveInProgress = true;

		Debug.Log("Score Combo Zeroed");
		scoreCombo = 0;
	}

	public void OnMoveEnded()
	{
		ResetPossibleMoves();
		MovePathManager.Instance.EndPath();

		// Spawn the needed amount of pieces.
		for (int i = 0; i < numPiecesToSpawn; i++)
		{
			PlaceNextRandomPiece();
		}

		numPiecesToSpawn = 0;

		// Check for Game Over after every move.
		CheckForGameEnd();

		moveInProgress = false;
	}

	void CheckForGameEnd()
	{
		if (IsGameOver())
		{
			StartCoroutine(GameEndRoutine());
		}
	}

	bool IsGameOver()
	{
		bool gameOver = true;

		for (int i = 0; i < pieces.GetLength(0); i++)
		{
			for (int j = 0; j < pieces.GetLength(1); j++)
			{
				if (pieces[i, j] != null && !pieces[i, j].GetMoveDisabled()) 
				{
					gameOver = false;
				}
			}
		}

		return gameOver;
	}

	IEnumerator GameEndRoutine()
	{
		GameManager.Instance.ShrinkMeToSlit(pieceViewerObj, 0f, Ease.OutQuint);

		yield return new WaitForSeconds(0.5f);

		StartCoroutine(DropPieces());
		StartCoroutine(DropTiles());

		yield return new WaitForSeconds(2f);

		score.SubmitScore();
		highScore.PullHighScore();
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
		for (int i = 0; i < tileObjects.GetLength(0); i++)
		{
			for (int j = 0; j < tileObjects.GetLength(1); j++)
			{
				tileObjects[i, j].GetComponent<Tile>().HideMove();

				if (pieces[i, j] != null)
				{
					pieces[i, j].SetPieceDown();
					pieces[i, j].SetPushPotential(false);
				}
			}
		}

		if (currentSelectedPiece != null)
		{
			currentSelectedPiece.SetPieceDown();
			currentSelectedPiece.SetPushPotential(false);
			currentSelectedPiece = null;
		}
		
		currentPossibleMoves = null;
		currentMovePiece = null;
		currentMoveTile = null;
	}

	IntVector2 RandomCoordinates()
	{
		return new IntVector2(Random.Range(0, Constants.I.GridSize.x), Random.Range(0, Constants.I.GridSize.y));
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

	private void PlacePieces()
	{
		// Spawn all the starting pieces.
		for (int i = 0; i < Constants.I.StartingPieceCount; i++)
		{
			PlaceRandomPiece();
		}
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
}