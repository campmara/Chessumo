﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mara.MrTween;

public abstract class Piece : MonoBehaviour {
    public Color FullColor { get { return fullColor; } }
    public Color SubduedColor { get { return subduedColor; } }
    [Header("Piece Colors"), SerializeField] private Color fullColor = Color.white;
    [SerializeField] private Color subduedColor = Color.white;

    [Header("Other Variables"), SerializeField] Color tint = Color.white;
    [SerializeField] Color disabledTint = Color.black;
    [ReadOnly] public bool potentialPush = false;

    public int pieceID = -1;

    private const EaseType moveEase = EaseType.Linear;

    public InitialMove Moveset { get { return moveset; } }
    protected InitialMove moveset;

    // Array of coordinate offsets that define the moves a piece can make.
    protected Vector2Int[] moveOffsets;

    protected int moveMagnitude = 1;
    [SerializeField] protected Vector2Int currentCoordinates;
    protected Game parentGame;

    protected bool moveDisabled = false;

    SpriteRenderer sprite;

    protected virtual void Awake() {
        sprite = GetComponentInChildren<SpriteRenderer>();
        sprite.color = tint;
    }

    public Vector2Int GetCoordinates() {
        return currentCoordinates;
    }

    public void SetCoordinates(Vector2Int coordinates) {
        parentGame.UpdatePieceCoordinates(this, currentCoordinates, coordinates);
        currentCoordinates = coordinates;
    }

    public void SetInfo(Vector2Int coords, Game game) {
        currentCoordinates = new Vector2Int(coords.x, coords.y);
        parentGame = game;
    }

    public void SetSortingLayer(string layer) {
        sprite.sortingLayerName = layer;
    }

    public void SetPushPotential(bool hasPotential) {
        potentialPush = hasPotential;
    }

    // For when it's underneath a tile that's highlighted.
    public void PickPieceUp() {
        sprite.transform.localPosition += Vector3.up * 0.1f;
    }

    public void SetPieceDown() {
        sprite.transform.localPosition = Vector3.zero;
    }

    void OnEnable() {
        // Spawn in 1 second after the tiles do.
        GameManager.Instance.GrowMe(this.gameObject);
    }

    public virtual List<Vector2Int> GetPossibleMoves() {
        List<Vector2Int> list = new List<Vector2Int>();

        for (int i = 0; i < moveset.moveList.Count; i++) {
            if (moveset.moveList[i].isPossibleEnd) {
                list.Add(moveset.moveList[i].coordinates);
            }
        }

        return list;
    }

    public virtual void DetermineMoveset() { }

    protected MoveUp GetUp(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(0, 1);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveUp[] moves = new MoveUp[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveUp up = new MoveUp();
                up.coordinates = newCoords;
                up.moveOffset = offset;
                up.isPossibleEnd = true;
                moves[i] = up;
                if (i == 0) {
                    up.reverse = init;
                } else {
                    moves[i - 1].up = up;
                    up.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(up);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected MoveDown GetDown(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(0, -1);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveDown[] moves = new MoveDown[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveDown down = new MoveDown();
                down.coordinates = newCoords;
                down.moveOffset = offset;
                down.isPossibleEnd = true;
                moves[i] = down;
                if (i == 0) {
                    down.reverse = init;
                } else {
                    moves[i - 1].down = down;
                    down.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(down);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected MoveLeft GetLeft(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(-1, 0);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveLeft[] moves = new MoveLeft[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveLeft left = new MoveLeft();
                left.coordinates = newCoords;
                left.moveOffset = offset;
                left.isPossibleEnd = true;
                moves[i] = left;
                if (i == 0) {
                    left.reverse = init;
                } else {
                    moves[i - 1].left = left;
                    left.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(left);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected MoveRight GetRight(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(1, 0);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveRight[] moves = new MoveRight[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveRight right = new MoveRight();
                right.coordinates = newCoords;
                right.moveOffset = offset;
                right.isPossibleEnd = true;
                moves[i] = right;
                if (i == 0) {
                    right.reverse = init;
                } else {
                    moves[i - 1].right = right;
                    right.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(right);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected virtual MoveUpRight GetUpRight(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(1, 1);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveUpRight[] moves = new MoveUpRight[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveUpRight up_right = new MoveUpRight();
                up_right.coordinates = newCoords;
                up_right.moveOffset = offset;
                up_right.isPossibleEnd = true;
                moves[i] = up_right;
                if (i == 0) {
                    up_right.reverse = init;
                } else {
                    moves[i - 1].up_right = up_right;
                    up_right.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(up_right);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected virtual MoveDownRight GetDownRight(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(1, -1);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveDownRight[] moves = new MoveDownRight[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveDownRight down_right = new MoveDownRight();
                down_right.coordinates = newCoords;
                down_right.moveOffset = offset;
                down_right.isPossibleEnd = true;
                moves[i] = down_right;
                if (i == 0) {
                    down_right.reverse = init;
                } else {
                    moves[i - 1].down_right = down_right;
                    down_right.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(down_right);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected virtual MoveDownLeft GetDownLeft(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(-1, -1);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveDownLeft[] moves = new MoveDownLeft[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveDownLeft down_left = new MoveDownLeft();
                down_left.coordinates = newCoords;
                down_left.moveOffset = offset;
                down_left.isPossibleEnd = true;
                moves[i] = down_left;
                if (i == 0) {
                    down_left.reverse = init;
                } else {
                    moves[i - 1].down_left = down_left;
                    down_left.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(down_left);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    protected virtual MoveUpLeft GetUpLeft(ref InitialMove init) {
        Vector2Int offset = new Vector2Int(-1, 1);
        Vector2Int newCoords = currentCoordinates + offset;
        MoveUpLeft[] moves = new MoveUpLeft[moveMagnitude];

        for (int i = 0; i < moveMagnitude; i++) {
            if (parentGame.IsWithinBounds(newCoords)) {
                MoveUpLeft up_left = new MoveUpLeft();
                up_left.coordinates = newCoords;
                up_left.moveOffset = offset;
                up_left.isPossibleEnd = true;
                moves[i] = up_left;
                if (i == 0) {
                    up_left.reverse = init;
                } else {
                    moves[i - 1].up_left = up_left;
                    up_left.reverse = moves[i - 1];
                }

                // Add this move to the list of this moveset's moves.
                init.moveList.Add(up_left);

                // Increment coords to check the next space up.
                newCoords = newCoords + offset;
            } else {
                break;
            }
        }

        return moves[0];
    }

    public virtual void MoveTo(Vector2Int coordinates, bool pushed, int distFromPushingPiece, int numPushedPieces) {
        // This is the core disable / pushing logic.
        if (pushed) {
            if (moveDisabled) {
                SetMoveDisabled(false);
            }
        }

        SetSortingLayer("Moving Pieces");

        // Determine how many times we must repeat the movement to get to the desired point.
        Vector2Int diff = coordinates - GetCoordinates();
        //float pushWaitTime = CalculatePushWaitTime(distFromPushingPiece, numPushedPieces);
        float pushWaitTime = parentGame.GetWaitTime();

        if (diff.x != 0 && diff.y == 0) {
            // Move horizontally.
            StartCoroutine(MoveX(diff.x, pushed, pushWaitTime));
        } else if (diff.y != 0 && diff.x == 0) {
            // Move vertically.
            StartCoroutine(MoveY(diff.y, pushed, pushWaitTime));
        } else if (diff.x != 0 && diff.y != 0 && Mathf.Abs(diff.x) == Mathf.Abs(diff.y)) {
            // Move diagonally.
            StartCoroutine(MoveDiagonally(diff.x, diff.y, pushed, pushWaitTime));
        }
    }

    public bool GetMoveDisabled() {
        return moveDisabled;
    }

    public void SetMoveDisabled(bool disabled) {
        moveDisabled = disabled;
        if (disabled) {
            sprite.ColorTo(disabledTint, 0.6f).Start();
            //sprite.color = disabledTint;
        } else {
            sprite.ColorTo(tint, 0.6f).Start();
            //sprite.color = tint;
        }
    }

    protected IEnumerator MoveXThenY(int xDistance, int yDistance, bool pushed) {
        // X MOVEMENT

        int sign = Mathf.FloorToInt(Mathf.Sign(xDistance));
        Vector2Int nextCoordinates = currentCoordinates + new Vector2Int(xDistance, 0);
        Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        Vector3 dir = new Vector3((float)sign, 0f, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(sign, 0), xDistance);
        SetCoordinates(nextCoordinates);

        ITween<Vector3> tween;

        for (int i = Mathf.Abs(xDistance); i > 0; i--) {
            tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        parentGame.ResetWaitTimeOnChangeDirection();

        // Y MOVEMENT

        sign = Mathf.FloorToInt(Mathf.Sign(yDistance));
        nextCoordinates = currentCoordinates + new Vector2Int(0, yDistance);
        convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        dir = new Vector3(0f, (float)sign, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(0, sign), yDistance);
        SetCoordinates(nextCoordinates);

        for (int i = Mathf.Abs(yDistance); i > 0; i--) {
            tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        SetSortingLayer("Pieces");

        if (!pushed) {
            parentGame.OnMoveEnded();
        }
    }

    protected IEnumerator MoveYThenX(int xDistance, int yDistance, bool pushed) {
        // Y MOVEMENT

        int sign = Mathf.FloorToInt(Mathf.Sign(yDistance));
        Vector2Int nextCoordinates = currentCoordinates + new Vector2Int(0, yDistance);
        Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        Vector3 dir = new Vector3(0f, (float)sign, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(0, sign), yDistance);
        SetCoordinates(nextCoordinates);

        ITween<Vector3> tween;

        for (int i = Mathf.Abs(yDistance); i > 0; i--) {
            tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        parentGame.ResetWaitTimeOnChangeDirection();

        // X MOVEMENT

        sign = Mathf.FloorToInt(Mathf.Sign(xDistance));
        nextCoordinates = currentCoordinates + new Vector2Int(xDistance, 0);
        convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        dir = new Vector3((float)sign, 0f, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(sign, 0), xDistance);
        SetCoordinates(nextCoordinates);

        for (int i = Mathf.Abs(xDistance); i > 0; i--) {
            tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        SetSortingLayer("Pieces");

        if (!pushed) {
            parentGame.OnMoveEnded();
        }
    }

    protected IEnumerator MoveX(int distance, bool pushed, float pushWaitTime) {
        int sign = Mathf.FloorToInt(Mathf.Sign(distance));
        Vector2Int nextCoordinates = currentCoordinates + new Vector2Int(distance, 0);
        Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        Vector3 dir = new Vector3((float)sign, 0f, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(sign, 0), distance);
        SetCoordinates(nextCoordinates);

        if (pushed) {
            yield return new WaitForSeconds(pushWaitTime);
        }

        for (int i = Mathf.Abs(distance); i > 0; i--) {
            ITween<Vector3> tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        SetSortingLayer("Pieces");

        if (!pushed) {
            parentGame.OnMoveEnded();
        }
    }

    protected IEnumerator MoveY(int distance, bool pushed, float pushWaitTime) {
        int sign = Mathf.FloorToInt(Mathf.Sign(distance));
        Vector2Int nextCoordinates = currentCoordinates + new Vector2Int(0, distance);
        Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        Vector3 dir = new Vector3(0f, (float)sign, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(0, sign), distance);
        SetCoordinates(nextCoordinates);

        if (pushed) {
            yield return new WaitForSeconds(pushWaitTime);
        }

        for (int i = Mathf.Abs(distance); i > 0; i--) {
            ITween<Vector3> tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        SetSortingLayer("Pieces");

        if (!pushed) {
            parentGame.OnMoveEnded();
        }
    }

    protected IEnumerator MoveDiagonally(int xDistance, int yDistance, bool pushed, float pushWaitTime) {
        int xSign = Mathf.FloorToInt(Mathf.Sign(xDistance));
        int ySign = Mathf.FloorToInt(Mathf.Sign(yDistance));
        Vector2Int nextCoordinates = currentCoordinates + new Vector2Int(xDistance, yDistance);
        Vector3 convertedPosition = GameManager.Instance.CoordinateToPosition(nextCoordinates);
        Vector3 newPos = new Vector3(convertedPosition.x, convertedPosition.y, transform.position.z);
        Vector3 dir = new Vector3((float)xSign, (float)ySign, 0f);

        parentGame.OnPieceMove(this, new Vector2Int(xSign, ySign), xDistance);
        SetCoordinates(nextCoordinates);

        if (pushed) {
            yield return new WaitForSeconds(pushWaitTime);
        }

        for (int i = Mathf.Abs(xDistance); i > 0; i--) {
            ITween<Vector3> tween = transform.PositionTo(transform.position + dir, Constants.I.PieceMoveTime).SetEaseType(moveEase);
            tween.Start();
            yield return tween.WaitForCompletion();
        }

        SetSortingLayer("Pieces");

        if (!pushed) {
            parentGame.OnMoveEnded();
        }
    }

    public float CalculatePushWaitTime(int distFromPushingPiece, int numPushedPieces) {
        int clampPushed = Mathf.Clamp(numPushedPieces - 1, 0, numPushedPieces + 1);
        int clampDist = Mathf.Clamp((distFromPushingPiece - 1) - clampPushed, 0, distFromPushingPiece + 1);
        return Constants.I.PieceMoveTime * clampDist;
    }
}