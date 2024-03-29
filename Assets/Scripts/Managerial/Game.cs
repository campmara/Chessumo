using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mara.MrTween;

public class Game : MonoBehaviour {
    private GameObject topUIBarObj;
    private GameObject scoresButtonObj;
    private GameObject menuButtonObj;
    private GameObject resetButtonObj;

    private GameObject[,] tileObjects;
    private Piece[,] pieces;
    private int nextRandomPieceID;
    private int numPiecesToSpawn = 0;

    private int scoreCombo;

    private GameObject pieceViewerObj;
    private NextPieceViewer pieceViewer;

    private Vector2Int nextRandomCoords;

    private bool moveInProgress = false;

    [ReadOnly, SerializeField] private Piece currentSelectedPiece;
    private List<Vector2Int> currentPossibleMoves;
    private Piece currentMovePiece = null;
    private Tile currentMoveTile = null;

    private void Awake() {
        tileObjects = new GameObject[Constants.I.GridSize.x, Constants.I.GridSize.y];
        pieces = new Piece[Constants.I.GridSize.x, Constants.I.GridSize.y];

        currentSelectedPiece = null;
        currentPossibleMoves = new List<Vector2Int>();
    }

    public void UpdatePieceCoordinates(Piece piece, Vector2Int oldCoordinates, Vector2Int newCoordinates) {
        pieces[oldCoordinates.x, oldCoordinates.y] = null;
        pieces[newCoordinates.x, newCoordinates.y] = piece;
    }

    public bool IsWithinBounds(Vector2Int coordinates) {
        return (coordinates.x >= 0 && coordinates.x < Constants.I.GridSize.x) && (coordinates.y >= 0 && coordinates.y < Constants.I.GridSize.y);
    }

    public bool CoordsOccupied(Vector2Int coordinates) {
        return pieces[coordinates.x, coordinates.y] != null;
    }

    public bool CurrentIsKnight() {
        return currentSelectedPiece.GetType() == typeof(Knight);
    }

    public void Load() {
        StartCoroutine(SetupBoard());
    }

    IEnumerator SetupBoard() {
        AudioManager.Instance.PlayStartRelease();
        SetupPlayfield();

        yield return new WaitForSeconds(1.5f);

        AudioManager.Instance.PlayStartPiecesSpawn();
        PlacePieces();

        yield return new WaitForSeconds(1f);

        SetupPieceViewer();
    }

    void SetupPlayfield() {
        for (int x = 0; x < Constants.I.GridSize.x; x++) {
            for (int y = 0; y < Constants.I.GridSize.y; y++) {
                GameObject tileObj = Instantiate(GameManager.Instance.tilePrefab) as GameObject;
                Tile tile = tileObj.GetComponent<Tile>();
                tile.gameObject.name = "Tile (" + x + ", " + y + ")";
                tile.transform.parent = transform;

                tile.transform.position = GameManager.Instance.CoordinateToPosition(new Vector2Int(x, y));

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

    void SetupPieceViewer() {
        pieceViewerObj = Instantiate(GameManager.Instance.nextPieceViewerPrefab) as GameObject;
        pieceViewerObj.name = "Piece Viewer";
        pieceViewerObj.transform.parent = transform;
        //pieceViewerObj.transform.position = new Vector3(0f, Constants.I.ScoreRaisedY - 1f, 0f);
        pieceViewer = pieceViewerObj.GetComponent<NextPieceViewer>();

        DecideNextRandomPiece();
    }

    private IEnumerator TileGrowRoutine() {
        int[] order = new int[tileObjects.GetLength(0) * tileObjects.GetLength(1)];
        for (int i = 0; i < order.Length; i++) {
            order[i] = i;
        }
        // now shuffle
        for (int t = 0; t < order.Length; t++) {
            int tmp = order[t];
            int r = Random.Range(t, order.Length);
            order[t] = order[r];
            order[r] = tmp;
        }

        for (int i = 0; i < order.Length; i++) {
            int x = order[i] % tileObjects.GetLength(0);
            int y = order[i] / tileObjects.GetLength(1);

            Tile tile = tileObjects[x, y].GetComponent<Tile>();

            yield return new WaitForSeconds(Random.Range(0f, 0.1f));

            tile.transform.LocalScaleTo(new Vector3(1f, 1f, 1f), 1f)
                .SetEaseType(EaseType.BackOut)
                .Start();
        }
    }

#if UNITY_EDITOR

    private void Update() {
        if (moveInProgress) {
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            OnFingerDown();
        }

        if (currentSelectedPiece) {
            OnFingerMove();
        }

        if (Input.GetMouseButtonUp(0)) {
            OnFingerUp();
        }
    }

    // -------------------------------------------------------
    // INPUT CALLBACKS
    // -------------------------------------------------------

    private void OnFingerDown() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            HandleDownRayHit(hit);
        }
    }

    private void OnFingerMove() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            HandleMoveRayHit(hit);
        }
    }

    private void OnFingerUp() {
        // Try and initiate the move.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            HandleUpRayHit(hit);
        } else {
            ResetPossibleMoves();
        }
    }

#elif UNITY_IOS || UNITY_ANDROID
    private void Update()
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

    private void OnFingerDown(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            HandleDownRayHit(hit);
        }
    }

    private void OnFingerMove(Touch touch)
    {
        Ray ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            HandleMoveRayHit(hit);
        }
    }

    private void OnFingerUp(Touch touch)
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

    private void HandleDownRayHit(RaycastHit hit) {
        if (UIManager.Instance.IsMenuOpen()) return;

        if (hit.collider.GetComponent<Piece>()) {
            Piece piece = hit.collider.GetComponent<Piece>();

            if (!piece.potentialPush && !piece.GetMoveDisabled()) {
                //ResetPossibleMoves();
                SelectPiece(piece);
            }
        }
    }

    private Vector2Int lastMoveCoords;

    private void HandleMoveRayHit(RaycastHit hit) {
        if (UIManager.Instance.IsMenuOpen()) return;

        if (hit.collider.GetComponent<Piece>() == currentSelectedPiece) {
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
        if (tile != null) {
            // Check to make sure we're processing a new tile and not a repeat one.
            if (tile == currentMoveTile) {
                return;
            }

            if (!CheckCoordsWithinPossibleMoves(tile.GetCoordinates())) {
                return;
            }


            if (CurrentIsKnight()) {
                Move move = currentSelectedPiece.Moveset.DetermineKnightMove(tile.GetCoordinates(), tile.GetCoordinates() - lastMoveCoords);
                if (currentSelectedPiece.GetComponent<Knight>().IsValidMove(move)) {
                    DrawMove(move);
                }
            } else {
                Move move = currentSelectedPiece.Moveset.DetermineMove(tile.GetCoordinates());
                DrawMove(move);
            }

            currentMoveTile = tile;
            lastMoveCoords = tile.GetCoordinates();
            currentMovePiece = null;
        } else if (piece != null) {
            // Check to make sure we're processing a new tile and not a repeat one.
            if (piece == currentMovePiece) {
                return;
            }

            if (!CheckCoordsWithinPossibleMoves(piece.GetCoordinates())) {
                return;
            }

            if (CurrentIsKnight()) // Check to make sure this is a valid movement.
            {
                Move move = currentSelectedPiece.Moveset.DetermineKnightMove(piece.GetCoordinates(), piece.GetCoordinates() - lastMoveCoords);
                if (currentSelectedPiece.GetComponent<Knight>().IsValidMove(move)) {
                    DrawMove(move);
                }
            } else {
                Move move = currentSelectedPiece.Moveset.DetermineMove(piece.GetCoordinates());
                DrawMove(move);
            }

            currentMovePiece = piece;
            lastMoveCoords = piece.GetCoordinates();
            currentMoveTile = tileObjects[piece.GetCoordinates().x, piece.GetCoordinates().y].GetComponent<Tile>();
        }
    }

    private void HandleUpRayHit(RaycastHit hit) {
        if (UIManager.Instance.IsMenuOpen()) return;

        if (hit.collider.GetComponent<Piece>()) // up on a piece
        {
            Piece piece = hit.collider.GetComponent<Piece>();

            if (piece.potentialPush && MoveIsOnPath(piece.GetCoordinates())) {
                if (CurrentIsKnight()) {
                    Vector2Int dir = tilesToRaise[0] - currentSelectedPiece.GetCoordinates();
                    currentSelectedPiece.GetComponent<Knight>().SetInitialDirection(dir);
                }

                currentSelectedPiece.MoveTo(piece.GetCoordinates(), false, 0, 0);
                OnMoveInitiated();
            } else {
                AudioManager.Instance.PlayPiecePickup();
                ResetPossibleMoves();
            }
        } else if (hit.collider.GetComponent<Tile>()) // up on a tile
          {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (currentSelectedPiece && tile.GetCoordinates() == currentSelectedPiece.GetCoordinates()) {
                AudioManager.Instance.PlayPiecePickup();
                ResetPossibleMoves();
            } else if (currentSelectedPiece && tile.GetState() == TileState.DRAWN) {
                if (CurrentIsKnight()) {
                    Vector2Int dir = tilesToRaise[0] - currentSelectedPiece.GetCoordinates();
                    currentSelectedPiece.GetComponent<Knight>().SetInitialDirection(dir);
                }

                // We just made a move!!! omg !!!!
                currentSelectedPiece.MoveTo(tile.GetCoordinates(), false, 0, 0);
                OnMoveInitiated();
            } else if (currentSelectedPiece && tile.GetState() != TileState.DRAWN) {
                AudioManager.Instance.PlayPiecePickup();
                ResetPossibleMoves();
            }
        }
    }

    private bool MoveIsOnPath(Vector2Int coords) {
        for (int i = 0; i < tilesToRaise.Length; i++) {
            if (tilesToRaise[i] == coords) return true;
        }

        return false;
    }

    private void BeginDrawingMove() {
        tilesToRaise = null;
        MovePathManager.Instance.BeginPath(currentSelectedPiece.GetCoordinates());
    }

    private Vector2Int[] tilesToRaise;

    private void DrawMove(Move move) {
        ClearPreviousDraw();

        // Handle new path.
        if (CurrentIsKnight()) {
            currentSelectedPiece.GetComponent<Knight>().UpdateKnightMove(move);

            Vector2Int[] temp = MovePathManager.Instance.CalculateKnightPath(tilesToRaise, move);
            tilesToRaise = temp;
        } else {
            tilesToRaise = MovePathManager.Instance.CalculatePath(move);
        }

        if (tilesToRaise == null) return;

        for (int i = 0; i < tilesToRaise.Length; i++) {
            Tile tile = tileObjects[tilesToRaise[i].x, tilesToRaise[i].y].GetComponent<Tile>();
            if (tile.GetState() == TileState.KNIGHT_TRAVERSABLE) {
                tile.SetState(TileState.KNIGHT_TRAVERSED, currentSelectedPiece.FullColor);
            } else {
                tile.SetState(TileState.DRAWN, currentSelectedPiece.FullColor);
                if (pieces[tilesToRaise[i].x, tilesToRaise[i].y] != null) pieces[tilesToRaise[i].x, tilesToRaise[i].y].PickPieceUp();
            }
        }
    }

    private void ClearPreviousDraw() {
        if (tilesToRaise != null) {
            for (int i = 0; i < tilesToRaise.Length; i++) {
                Tile tile = tileObjects[tilesToRaise[i].x, tilesToRaise[i].y].GetComponent<Tile>();
                if (tile.GetState() == TileState.KNIGHT_TRAVERSED) {
                    tile.SetState(TileState.KNIGHT_TRAVERSABLE, Color.black);
                } else {
                    tile.SetState(TileState.POSSIBLE, currentSelectedPiece.SubduedColor);
                }
                if (pieces[tilesToRaise[i].x, tilesToRaise[i].y] != null) pieces[tilesToRaise[i].x, tilesToRaise[i].y].SetPieceDown();
            }
        }
    }

    private void DrawReturnedToStartPiece() {
        if (tilesToRaise != null) {
            for (int i = 0; i < tilesToRaise.Length; i++) {
                Tile tile = tileObjects[tilesToRaise[i].x, tilesToRaise[i].y].GetComponent<Tile>();
                if (tile.GetState() == TileState.KNIGHT_TRAVERSED) {
                    tile.SetState(TileState.KNIGHT_TRAVERSABLE, Color.black);
                } else {
                    tile.SetState(TileState.POSSIBLE, currentSelectedPiece.SubduedColor);
                }
                if (pieces[tilesToRaise[i].x, tilesToRaise[i].y] != null) pieces[tilesToRaise[i].x, tilesToRaise[i].y].SetPieceDown();
            }
        }

        tilesToRaise = null;
        lastMoveCoords = currentSelectedPiece.GetCoordinates();
        if (CurrentIsKnight()) {
            currentSelectedPiece.GetComponent<Knight>().ResetKnight();
            currentMovePiece = null;
            currentMoveTile = null;
        }
    }

    private void EndDrawingMove() {
        for (int i = 0; i < tileObjects.GetLength(0); i++) {
            for (int j = 0; j < tileObjects.GetLength(1); j++) {
                tileObjects[i, j].GetComponent<Tile>().SetState(TileState.DEFAULT, Color.black);

                if (pieces[i, j] != null) {
                    pieces[i, j].SetPieceDown();
                    pieces[i, j].SetPushPotential(false);
                }
            }
        }

        MovePathManager.Instance.EndPath();
    }

    private bool CheckCoordsWithinPossibleMoves(Vector2Int coords) {
        if (CurrentIsKnight()) {
            Vector2Int[] secondaryMoves = currentSelectedPiece.GetComponent<Knight>().GetSecondaryMoves();

            for (int i = 0; i < currentPossibleMoves.Count; i++) {
                if (currentPossibleMoves[i] == coords) {
                    return true;
                }
            }

            for (int i = 0; i < secondaryMoves.Length; i++) {
                if (secondaryMoves[i] == coords) {
                    return true;
                }
            }
        } else {
            for (int i = 0; i < currentPossibleMoves.Count; i++) {
                if (currentPossibleMoves[i] == coords) {
                    return true;
                }
            }
        }

        return false;
    }

    private float waitTime = 0f;
    public float GetWaitTime() {
        return waitTime;
    }
    public void IncrementWaitTime() {
        waitTime += Constants.I.PieceMoveTime;
    }
    // for use specifically with MoveXThenY functions
    public void ResetWaitTimeOnChangeDirection() {
        waitTime = 0f;
    }

    public void OnPieceMove(Piece piece, Vector2Int direction, int distance) {
        Vector2Int oldCoordinates = piece.GetCoordinates();
        Vector2Int currentCheckingCoords = oldCoordinates;
        int absDist = Mathf.Abs(distance);
        int distFromPushingPiece = 0;
        int numPushedPieces = 0;

        for (int i = 0; i < absDist; i++) {
            currentCheckingCoords += direction;
            distFromPushingPiece++;

            if (pieces[currentCheckingCoords.x, currentCheckingCoords.y] != null) {
                int pushDistance = (absDist - distFromPushingPiece) + 1;
                Vector2Int push = currentCheckingCoords + direction * pushDistance;
                numPushedPieces++;

                if (IsWithinBounds(push)) {
                    // Push in the grid.
                    pieces[currentCheckingCoords.x, currentCheckingCoords.y].MoveTo(push, true, distFromPushingPiece, numPushedPieces);
                    break;
                } else {
                    // PUSH OFF THE GRID.
                    PieceOffGrid(pieces[currentCheckingCoords.x, currentCheckingCoords.y], push, absDist, distFromPushingPiece, numPushedPieces);
                    pieces[currentCheckingCoords.x, currentCheckingCoords.y] = null;
                }
            } else {
                IncrementWaitTime();
            }
        }
    }

    private void PieceOffGrid(Piece piece, Vector2Int pushCoordinates, int travelDist, int distFromPushingPiece, int numPushedPieces) {
        numPiecesToSpawn++;

        Vector2 offGridPosition = GameManager.Instance.CoordinateToPosition(pushCoordinates);

        StartCoroutine(MovePieceOffGrid(piece, offGridPosition, travelDist, distFromPushingPiece, numPushedPieces));
    }

    protected IEnumerator MovePieceOffGrid(Piece piece, Vector2 position, int travelDist, int distFromPushingPiece, int numPushedPieces) {
        //float waitTime = piece.CalculatePushWaitTime(distFromPushingPiece, numPushedPieces);
        //yield return new WaitForSeconds(waitTime);
        yield return new WaitForSeconds(GetWaitTime());

        GameObject pieceObj = piece.gameObject;

        Vector3 oldPos = GameManager.Instance.CoordinateToPosition(piece.GetCoordinates());
        oldPos.z = pieceObj.transform.position.z;
        Vector3 newPos = new Vector3(position.x, position.y, pieceObj.transform.position.z);
        Vector3 dir = (newPos - oldPos).normalized;

        ITween<Vector3> vector3Tween;

        for (int i = travelDist - Mathf.Clamp(distFromPushingPiece - 1, 0, distFromPushingPiece + 1); i > 0; i--) {
            if (i == 1) {
                AudioManager.Instance.PlayPieceOffGrid();
            }

            vector3Tween = pieceObj.transform.PositionTo(piece.transform.position + dir, Constants.I.PieceMoveTime)
                .SetEaseType(EaseType.Linear);
            vector3Tween.Start();

            yield return vector3Tween.WaitForCompletion();
        }

        piece.SetSortingLayer("Falling Pieces");
        Destroy(piece); // Destroy the piece class so it won't get touched.

        ITween<float> floatTween = pieceObj.transform.YPositionTo(-6f, 0.75f).SetEaseType(EaseType.CubicIn);
        floatTween.Start();
        yield return floatTween.WaitForCompletion();

        // Score a point and handle the score combo.
        HandleScorePoint(piece);

        Destroy(pieceObj);
    }

    private void HandleScorePoint(Piece piece) {
        Debug.Log("Score Combo Incremented, Current Number: " + scoreCombo);
        scoreCombo++;

        if (scoreCombo >= Constants.I.GridSize.x - 1) {
            GameManager.Instance.scoreEffect.OnThreeScored(piece.FullColor);
            UIManager.Instance.ScorePoints(Constants.I.ScoreThreeAmount);
        } else if (scoreCombo >= 2) {
            GameManager.Instance.scoreEffect.OnTwoScored(piece.FullColor);
            UIManager.Instance.ScorePoints(Constants.I.ScoreTwoAmount);
        } else if (scoreCombo >= 1) {
            GameManager.Instance.scoreEffect.OnOneScored(piece.FullColor);
            UIManager.Instance.ScorePoints(Constants.I.ScoreOneAmount);
        }
    }

    private void SelectPiece(Piece piece) {
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
        for (int i = 0; i < currentPossibleMoves.Count; i++) {
            Vector2Int move = currentPossibleMoves[i];
            if (IsWithinBounds(move)) {
                tileObjects[move.x, move.y].GetComponent<Tile>().SetState(TileState.POSSIBLE, currentSelectedPiece.SubduedColor);

                if (pieces[move.x, move.y] != null) {
                    //pieces[move.x, move.y].PickPieceUp();
                    pieces[move.x, move.y].SetPushPotential(true);
                }
            }
        }

        // Handle Knight selection. We gotta get its secondary moves and highlight those too.
        if (CurrentIsKnight()) {
            Vector2Int[] secondaryMoves = currentSelectedPiece.GetComponent<Knight>().GetSecondaryMoves();

            for (int i = 0; i < secondaryMoves.Length; i++) {
                Vector2Int move = secondaryMoves[i];
                if (IsWithinBounds(move)) {
                    tileObjects[move.x, move.y].GetComponent<Tile>().SetState(TileState.KNIGHT_TRAVERSABLE, Color.black);
                }
            }
        }
    }

    public void OnMoveInitiated() {
        moveInProgress = true;

        AudioManager.Instance.PlayMoveHit();

        Debug.Log("Score Combo Zeroed");
        scoreCombo = 0;
    }

    public void OnMoveEnded() {
        currentSelectedPiece.SetMoveDisabled(true);
        ResetPossibleMoves();

        waitTime = 0f;

        // Spawn the needed amount of pieces.
        for (int i = 0; i < numPiecesToSpawn; i++) {
            PlaceNextRandomPiece();
        }

        numPiecesToSpawn = 0;

        // Checks whether it should end the game or enable NPV movement.
        CheckPieceCount();

        moveInProgress = false;
    }

    private void CheckPieceCount() {
        int movablePieceCount = 0;

        for (int i = 0; i < pieces.GetLength(0); i++) {
            for (int j = 0; j < pieces.GetLength(1); j++) {
                if (pieces[i, j] != null && !pieces[i, j].GetMoveDisabled()) {
                    movablePieceCount++;
                }
            }
        }

        if (movablePieceCount <= 0) {
            StartCoroutine(GameEndRoutine());
        }
    }

    IEnumerator GameEndRoutine() {
        AudioManager.Instance.PlayGameOver();
        StartCoroutine(DropPieces());
        StartCoroutine(DropTiles());

        yield return new WaitForSeconds(1.44f);

        AudioManager.Instance.PlayNPVFade();
        pieceViewer.FadeOut();

        yield return new WaitForSeconds(4.5f);

        GameManager.Instance.OnGameEnd();
    }

    IEnumerator DropPieces() {
        for (int i = 0; i < pieces.GetLength(0); i++) {
            for (int j = 0; j < pieces.GetLength(1); j++) {
                if (pieces[i, j] != null) {
                    float duration = Random.Range(0.4f, 1.5f);

                    tileObjects[i, j].transform.YPositionTo(-10f, duration).SetEaseType(EaseType.QuintIn).Start();
                    pieces[i, j].transform.YPositionTo(-10f, duration).SetEaseType(EaseType.QuintIn).Start();

                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

    IEnumerator DropTiles() {
        for (int i = 0; i < tileObjects.GetLength(0); i++) {
            for (int j = 0; j < tileObjects.GetLength(1); j++) {
                if (pieces[i, j] == null) {
                    // If there are no pieces on this then we can drop.
                    tileObjects[i, j].transform.YPositionTo(-10f, Random.Range(0.4f, 1.5f)).SetEaseType(EaseType.QuintIn).Start();
                    yield return new WaitForSeconds(0.01f);
                }
            }
        }
    }

    public void Unload() {
        Destroy(gameObject);
    }

    public void ResetPossibleMoves() {
        EndDrawingMove();

        if (currentSelectedPiece != null) {
            if (CurrentIsKnight()) {
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

    List<Vector2Int> tempListUnoccupied = new List<Vector2Int>();
    Vector2Int RandomCoordinates() {
        tempListUnoccupied.Clear();
        for (int i = 0; i < tileObjects.GetLength(0); i++) {
            for (int j = 0; j < tileObjects.GetLength(1); j++) {
                Vector2Int tileCoords = tileObjects[i, j].GetComponent<Tile>().GetCoordinates();
                if (pieces[tileCoords.x, tileCoords.y] == null) {
                    tempListUnoccupied.Add(tileCoords);
                }
            }
        }

        return tempListUnoccupied[Random.Range(0, tempListUnoccupied.Count)];

        //return new Vector2Int(Random.Range(0, Constants.I.GridSize.x), Random.Range(0, Constants.I.GridSize.y));
    }

    Vector2Int RandomCoordinatesInColumn(int x) {
        int count = 0;
        for (int i = 0; i < tileObjects.GetLength(1); i++) {
            if (pieces[x, i] != null) {
                count++;
            }
        }

        if (count >= Constants.I.GridSize.y - 1) {
            return RandomCoordinates();
        } else {
            return new Vector2Int(x, Random.Range(0, Constants.I.GridSize.y));
        }
    }

    protected void PlaceRandomPiece() {
        int randID = Random.Range(Constants.I.MinPieceID, Constants.I.MaxPieceID + 1);
        Vector2Int randCoords = RandomCoordinates();

        while (pieces[randCoords.x, randCoords.y] != null) {
            randCoords = RandomCoordinates();
        }

        CreatePiece(randID, randCoords);
    }

    private void DecideNextRandomPiece() {
        nextRandomPieceID = Random.Range(Constants.I.MinPieceID, Constants.I.MaxPieceID + 1);
        pieceViewer.ShowPiece(nextRandomPieceID);

        nextRandomCoords = RandomCoordinates();

        // Position the piece viewer at the top above the column it needs to be at.
        pieceViewer.PositionAlongTop(nextRandomCoords);
    }

    protected void PlaceNextRandomPiece() {
        while (pieces[nextRandomCoords.x, nextRandomCoords.y] != null) {
            nextRandomCoords = RandomCoordinatesInColumn(nextRandomCoords.x);
        }

        CreatePiece(nextRandomPieceID, nextRandomCoords);
        DecideNextRandomPiece();
    }

    private void PlacePieces() {
        StartCoroutine(PlacePiecesRoutine());
    }

    private IEnumerator PlacePiecesRoutine() {
        float[] waitTimes = new float[Constants.I.StartingPieceCount];
        waitTimes[0] = 0.1f;
        waitTimes[1] = 0.1f;
        waitTimes[2] = 0.1f;
        waitTimes[3] = 0.2f;
        waitTimes[4] = 0f;

        // Spawn all the starting pieces.
        for (int i = 0; i < Constants.I.StartingPieceCount; i++) {
            PlaceRandomPiece();
            yield return new WaitForSeconds(waitTimes[i]);
        }
    }

    protected Piece CreatePiece(int pieceID, Vector2Int coords) {
        GameObject obj = Instantiate(GameManager.Instance.piecePrefabs[pieceID]);

        Piece piece = obj.GetComponent<Piece>();
        piece.gameObject.name = piece.name;
        piece.transform.parent = transform;
        piece.transform.position = new Vector3(tileObjects[coords.x, coords.y].transform.position.x, tileObjects[coords.x, coords.y].transform.position.y, piece.transform.position.z);
        piece.SetInfo(coords, this);

        pieces[coords.x, coords.y] = piece;

        return piece;
    }
}