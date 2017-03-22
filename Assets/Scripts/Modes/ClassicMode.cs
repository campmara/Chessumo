using UnityEngine;
using System.Collections;
using DG.Tweening;

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

	GameObject scoreObj;
	Score score;
	
	GameObject highScoreObj;
	HighScore highScore;

	GameObject pieceViewerObj;
	NextPieceViewer pieceViewer;

	GameObject endMessageObj;
	GameEndMessage endMessage;

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
		Coroutine boardRoutine = StartCoroutine(SetupBoard());

		SetupScore();
		SetupPieceViewer();
		SetupGameEndMessage();
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

	protected override void PlacePieces()
	{
		/*
		CreatePawn(1, 2);
		CreatePawn(2, 2);
		CreatePawn(3, 2);
		CreateKnight(1, 1);
		CreateRook(2, 1);
		CreateKnight(3, 1);
		*/

		PlaceRandomPiece();
		PlaceRandomPiece();
		PlaceRandomPiece();
		PlaceRandomPiece();
		PlaceRandomPiece();
		PlaceRandomPiece();
		PlaceRandomPiece();
	}

	protected override void DecideNextRandomPiece()
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

	void SetupScore()
	{
		scoreObj = Instantiate(GameManager.Instance.scorePrefab) as GameObject;
		scoreObj.name = "Score";
		scoreObj.transform.parent = transform;
		scoreObj.transform.position = new Vector3(0f, Constants.SCORE_RAISED_Y, 0f);
		score = scoreObj.GetComponent<Score>();
		score.Reset();

		highScoreObj = Instantiate(GameManager.Instance.highScorePrefab) as GameObject;
		highScoreObj.name = "High Score";
		highScoreObj.transform.parent = transform;
		highScoreObj.transform.position = new Vector3(0f, Constants.SCORE_RAISED_Y, 0f);
		highScore = highScoreObj.GetComponent<HighScore>();
		highScore.PullHighScore();
	}

	void SetupPieceViewer()
	{
		pieceViewerObj = Instantiate(GameManager.Instance.nextPieceViewerPrefab) as GameObject;
		pieceViewerObj.name = "Piece Viewer";
		pieceViewerObj.transform.parent = transform;
		pieceViewerObj.transform.position = new Vector3(0f, Constants.SCORE_RAISED_Y - 1f, 0f);
		pieceViewer = pieceViewerObj.GetComponent<NextPieceViewer>();

		DecideNextRandomPiece();
	}

	void SetupGameEndMessage()
	{
		endMessageObj = Instantiate(GameManager.Instance.gameEndMessagePrefab) as GameObject;
		endMessageObj.name = "Game End Message";
		endMessageObj.transform.parent = transform;
		endMessage = endMessageObj.GetComponent<GameEndMessage>();
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
		GameManager.Instance.ShrinkMeToSlit(pieceViewerObj, 0f, Ease.InQuint);

		yield return new WaitForSeconds(0.5f);

		Coroutine pieceDropRoutine = StartCoroutine(DropPieces());
		Coroutine tileDropRoutine = StartCoroutine(DropTiles());

		yield return new WaitForSeconds(4f);

		// Lower the score and high score.
		//score.Lower();
		//highScore.Lower();
		yield return endMessage.Appear().WaitForCompletion();

		yield return new WaitForSeconds(0.5f);

		score.SubmitScore();
		highScore.PullHighScore();

		yield return new WaitForSeconds(1.5f);
		
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

		if (currentSelectedPiece)
		{
			
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
				if (!piece.potentialPush && !piece.GetMoveDisabled())
				{
					ResetPossibleMoves();

					SelectPiece(piece);
				}
				else if (!piece.potentialPush && piece.GetMoveDisabled())
				{
					GameManager.Instance.Deselect();
					currentSelectedPiece = null;
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
			}
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

	public override void OnMoveInitiated()
	{
		if (currentSelectedPiece.GetType() == typeof(Knight))
		{
			Knight knight = currentSelectedPiece.GetComponent<Knight>();

			if (knight.HasDirection())
			{
				knight.ResetKnight();
				GameManager.Instance.Deselect();
				currentSelectedPiece = null;
			}
			else
			{
				// Initiate another move for the knight.
				ResetPossibleMoves();
				SelectPiece(knight);
			}
		}
	}

	public override void OnMoveEnded()
	{
		GameManager.Instance.Deselect();
		currentSelectedPiece = null;

		// Check for Game Over after every move.
		CheckForGameEnd();
	}

	protected override void PieceOffGrid(Piece piece, IntVector2 pushCoordinates)
	{
		score.ScorePoint();

		Vector2 offGridPosition = GameManager.Instance.CoordinateToPosition(pushCoordinates);
		StartCoroutine(MovePieceOffGrid(piece, offGridPosition));

		PlaceNextRandomPiece();

		//PlaceRandomPiece();
	}

	//////////////////////////////////////////////////////////
	// DEACTIVATION
	//////////////////////////////////////////////////////////

	public override void Unload()
	{
		Destroy(gameObject);
	}
}
