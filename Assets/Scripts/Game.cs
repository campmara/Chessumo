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

	private GameObject scoreObj;
	private Score score;

	private int scoreCombo;
	
	private GameObject highScoreObj;
	private HighScore highScore;

	private GameObject pieceViewerObj;
	private NextPieceViewer pieceViewer;

	private bool moveInProgress = false;

	[ReadOnly, SerializeField] protected Piece currentSelectedPiece;

    private void Awake()
	{
		tileObjects = new GameObject[Constants.I.GridSize.x, Constants.I.GridSize.y];
		pieces = new Piece[Constants.I.GridSize.x, Constants.I.GridSize.y];

		currentSelectedPiece = null;
	}

	public void OnPieceMove(Piece piece, IntVector2 newCoordinates)
	{
		IntVector2 oldCoordinates = piece.GetCoordinates();

		if (pieces[newCoordinates.x, newCoordinates.y] != null)
		{
			// We're pushing! Yay!
			//Debug.Log("PUSHING PIECE");

			IntVector2 push = newCoordinates + (newCoordinates - oldCoordinates);

			if (IsWithinBounds(push))
			{
				// Push in the grid.
				//Debug.Log("Pushing within bounds.");
				pieces[newCoordinates.x, newCoordinates.y].MoveTo(push, true);
			}
			else
			{
				// PUSH OFF THE GRID.
				//Debug.Log("YOU JUST PUSHED A PIECE OFF THE GRID");
				PieceOffGrid(pieces[newCoordinates.x, newCoordinates.y], push);
				pieces[newCoordinates.x, newCoordinates.y] = null;
			}
		}
		else
		{
			//Debug.Log("Normal Move. No Push");
		}
	}

	private void PieceOffGrid(Piece piece, IntVector2 pushCoordinates)
	{
		score.ScorePoint();
		
		// Increment the score combo.
		IncrementScoreCombo();

		Vector2 offGridPosition = GameManager.Instance.CoordinateToPosition(pushCoordinates);
		StartCoroutine(MovePieceOffGrid(piece, offGridPosition));

		PlaceNextRandomPiece();

		//PlaceRandomPiece();
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

	protected IEnumerator MovePieceOffGrid(Piece piece, Vector2 position)
	{
		GameObject pieceObj = piece.gameObject;

		Vector3 newPos = new Vector3(position.x, position.y, pieceObj.transform.position.z);

		Tween tween = pieceObj.transform.DOMove(newPos, Constants.I.PieceMoveTime);
		yield return tween.WaitForCompletion();

		piece.HandleFallingSortingOrder();
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
		return (coordinates.x >= 0 && coordinates.x < Constants.I.GridSize.x) && (coordinates.y >= 0 && coordinates.y < Constants.I.GridSize.y);
	}

	public void Load() 
	{
		Coroutine boardRoutine = StartCoroutine(SetupBoard());

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
		//bool isAltColor = false;

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

				/*
				if (isAltColor)
					tile.SetColorAlternate();
				else
					tile.SetColorDefault();

				isAltColor = !isAltColor;
				*/

				tileObjects[x, y] = tileObj;
			}
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

	void CheckForGameEnd()
	{
		if (IsGameOver())
		{
			Coroutine endRoutine = StartCoroutine(GameEndRoutine());
		}
	}

	IEnumerator GameEndRoutine()
	{
		GameManager.Instance.ShrinkMeToSlit(pieceViewerObj, 0f, Ease.OutQuint);

		yield return new WaitForSeconds(0.5f);

		Coroutine pieceDropRoutine = StartCoroutine(DropPieces());
		Coroutine tileDropRoutine = StartCoroutine(DropTiles());

		yield return new WaitForSeconds(2f);

		// Lower the score and high score.
		//score.Lower();
		//highScore.Lower();
		//yield return endMessage.Appear().WaitForCompletion();

		//yield return new WaitForSeconds(0.5f);

		score.SubmitScore();
		highScore.PullHighScore();

		//yield return new WaitForSeconds(1.5f);
		
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

	//////////////////////////////////////////////////////////
	// UPDATE
	//////////////////////////////////////////////////////////

	void Update()
	{
		if (moveInProgress)
		{
			return;
		}

		if (Input.GetMouseButtonDown(0))
		{
			CheckRayMouse();
		}
#if UNITY_IPHONE
		else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
		{
			Touch touch = Input.GetTouch(0);
			CheckRayTap(touch);
		}
#endif
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
				ResetPossibleMoves();
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
				ResetPossibleMoves();
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
				ResetKnightIfNeeded();

				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
				ResetPossibleMoves();
			}
			else if (piece != currentSelectedPiece)
			{
				if (!piece.potentialPush && !piece.GetMoveDisabled())
				{	
					ResetKnightIfNeeded();

					ResetPossibleMoves();
					SelectPiece(piece);
				}
				else if (!piece.potentialPush && piece.GetMoveDisabled())
				{
					ResetKnightIfNeeded();

					GameManager.Instance.Deselect();
					currentSelectedPiece = null;
					ResetPossibleMoves();
				}
				else
				{
					// Force a move. We're probably going to be pushing too.
					IntVector2 coords = piece.GetCoordinates();

					currentSelectedPiece.MoveTo(coords, false);
					OnMoveInitiated();
				}
			}
		}
		else if (hit.collider.GetComponent<Tile>())
		{
			Tile tile = hit.collider.GetComponent<Tile>();

			if (currentSelectedPiece && tile.IsShowingMove())
			{
				// We just made a move!!! omg !!!!
				currentSelectedPiece.MoveTo(tile.GetCoordinates(), false);
				OnMoveInitiated();
			}
			else if (currentSelectedPiece && !tile.IsShowingMove())
			{
				if (currentSelectedPiece.GetType() == typeof(Knight))
				{
					currentSelectedPiece.GetComponent<Knight>().ResetKnight();
				}

				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
				ResetPossibleMoves();
			}
		}
	}

	void ResetKnightIfNeeded()
	{
		if (currentSelectedPiece && currentSelectedPiece.GetType() == typeof(Knight))
		{
			currentSelectedPiece.GetComponent<Knight>().ResetKnight();
		}
	}

	void SelectPiece(Piece piece)
	{
		currentSelectedPiece = piece;
		GameManager.Instance.SelectObject(currentSelectedPiece.transform);

		IntVector2[] possibleMoves = currentSelectedPiece.GetPossibleMoves();
		for (int i = 0; i < possibleMoves.Length; i++)
		{
			IntVector2 move = possibleMoves[i];
			if (IsWithinBounds(move))
			{
				tileObjects[move.x, move.y].GetComponent<Tile>().ShowMove();

				if (pieces[move.x, move.y] != null)
				{
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

		if (currentSelectedPiece.GetType() == typeof(Knight))
		{
			Knight knight = currentSelectedPiece.GetComponent<Knight>();

			if (knight.HasDirection())
			{
				knight.ResetKnight();
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
				ResetPossibleMoves();
			}
			else
			{
				// Initiate another move for the knight.
				ResetPossibleMoves();
				SelectPiece(knight);
				moveInProgress = false;
			}
		}
	}

	public void OnMoveEnded()
	{
		GameManager.Instance.Deselect();
		currentSelectedPiece = null;
		ResetPossibleMoves();

		// Check for Game Over after every move.
		CheckForGameEnd();

		moveInProgress = false;
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
					pieces[i, j].SetPushPotential(false);
				}
			}
		}
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
		/*
		CreatePawn(1, 2);
		CreatePawn(2, 2);
		CreatePawn(3, 2);
		CreateKnight(1, 1);
		CreateRook(2, 1);
		CreateKnight(3, 1);
		*/

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