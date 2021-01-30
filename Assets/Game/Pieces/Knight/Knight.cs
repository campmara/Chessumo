using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Knight : Piece {
    private IntVector2[] secondaryMoveOffsets;

    IntVector2 initialDirection;
    Move currentFingerMove;

    protected override void Awake() {
        base.Awake();

        //moveMagnitude = 1;

        initialDirection = IntVector2.NULL;
    }

    public void UpdateKnightMove(Move move) {
        currentFingerMove = move;
    }

    public bool IsValidMove(Move move) {
        if (currentFingerMove == null) return true;
        if (move == null) return false;

        if (move.GetType() == typeof(MoveUp) && currentFingerMove.GetType() == typeof(MoveUp)) {
            return (currentFingerMove as MoveUp).up == move;
        } else if (move.GetType() == typeof(MoveLeft) && currentFingerMove.GetType() == typeof(MoveUp)) {
            return (currentFingerMove as MoveUp).left == move;
        } else if (move.GetType() == typeof(MoveRight) && currentFingerMove.GetType() == typeof(MoveUp)) {
            return (currentFingerMove as MoveUp).right == move;
        } else if (move.GetType() == typeof(MoveDown) && currentFingerMove.GetType() == typeof(MoveDown)) {
            return (currentFingerMove as MoveDown).down == move;
        } else if (move.GetType() == typeof(MoveLeft) && currentFingerMove.GetType() == typeof(MoveDown)) {
            return (currentFingerMove as MoveDown).left == move;
        } else if (move.GetType() == typeof(MoveRight) && currentFingerMove.GetType() == typeof(MoveDown)) {
            return (currentFingerMove as MoveDown).right == move;
        } else if (move.GetType() == typeof(MoveLeft) && currentFingerMove.GetType() == typeof(MoveLeft)) {
            return (currentFingerMove as MoveLeft).left == move;
        } else if (move.GetType() == typeof(MoveUp) && currentFingerMove.GetType() == typeof(MoveLeft)) {
            return (currentFingerMove as MoveLeft).up == move;
        } else if (move.GetType() == typeof(MoveDown) && currentFingerMove.GetType() == typeof(MoveLeft)) {
            return (currentFingerMove as MoveLeft).down == move;
        } else if (move.GetType() == typeof(MoveRight) && currentFingerMove.GetType() == typeof(MoveRight)) {
            return (currentFingerMove as MoveRight).right == move;
        } else if (move.GetType() == typeof(MoveUp) && currentFingerMove.GetType() == typeof(MoveRight)) {
            return (currentFingerMove as MoveRight).up == move;
        } else if (move.GetType() == typeof(MoveDown) && currentFingerMove.GetType() == typeof(MoveRight)) {
            return (currentFingerMove as MoveRight).down == move;
        } else if (move == currentFingerMove.reverse) {
            // reverse.
            return true;
        } else {
            return false;
        }
    }

    public override void DetermineMoveset() {
        moveset = new InitialMove();

        // THIS IS INSANITY

        moveset.up = GetUp(ref moveset, currentCoordinates, false);
        if (moveset.up != null) {
            moveset.up.up = GetUp(ref moveset, moveset.up.coordinates, false);
            if (moveset.up.up != null) {
                moveset.up.up.left = GetLeft(ref moveset, moveset.up.up.coordinates, true);
                moveset.up.up.right = GetRight(ref moveset, moveset.up.up.coordinates, true);
                if (moveset.up.up.left == null && moveset.up.up.right == null) {
                    moveset.up.up = null;
                }
            }

            moveset.up.left = GetLeft(ref moveset, moveset.up.coordinates, false);
            if (moveset.up.left != null) {
                moveset.up.left.left = GetLeft(ref moveset, moveset.up.left.coordinates, true);
                if (moveset.up.left.left == null) {
                    moveset.up.left = null;
                }
            }

            moveset.up.right = GetRight(ref moveset, moveset.up.coordinates, false);
            if (moveset.up.right != null) {
                moveset.up.right.right = GetRight(ref moveset, moveset.up.right.coordinates, true);
                if (moveset.up.right.right == null) {
                    moveset.up.right = null;
                }
            }

            // Final Check to see if there are any upwards moves whatsoever.
            if (moveset.up.up == null && moveset.up.left == null && moveset.up.right == null) {
                moveset.up = null;
            }
        }





        moveset.down = GetDown(ref moveset, currentCoordinates, false);
        if (moveset.down != null) {
            moveset.down.down = GetDown(ref moveset, moveset.down.coordinates, false);
            if (moveset.down.down != null) {
                moveset.down.down.left = GetLeft(ref moveset, moveset.down.down.coordinates, true);
                moveset.down.down.right = GetRight(ref moveset, moveset.down.down.coordinates, true);
                if (moveset.down.down.left == null && moveset.down.down.right == null) {
                    moveset.down.down = null;
                }
            }

            moveset.down.left = GetLeft(ref moveset, moveset.down.coordinates, false);
            if (moveset.down.left != null) {
                moveset.down.left.left = GetLeft(ref moveset, moveset.down.left.coordinates, true);
                if (moveset.down.left.left == null) {
                    moveset.down.left = null;
                }
            }

            moveset.down.right = GetRight(ref moveset, moveset.down.coordinates, false);
            if (moveset.down.right != null) {
                moveset.down.right.right = GetRight(ref moveset, moveset.down.right.coordinates, true);
                if (moveset.down.right.right == null) {
                    moveset.down.right = null;
                }
            }

            // Final Check to see if there are any upwards moves whatsoever.
            if (moveset.down.down == null && moveset.down.left == null && moveset.down.right == null) {
                moveset.down = null;
            }
        }








        moveset.left = GetLeft(ref moveset, currentCoordinates, false);
        if (moveset.left != null) {
            moveset.left.left = GetLeft(ref moveset, moveset.left.coordinates, false);
            if (moveset.left.left != null) {
                moveset.left.left.up = GetUp(ref moveset, moveset.left.left.coordinates, true);
                moveset.left.left.down = GetDown(ref moveset, moveset.left.left.coordinates, true);

                if (moveset.left.left.up == null && moveset.left.left.down == null) {
                    moveset.left.left = null;
                }
            }

            moveset.left.up = GetUp(ref moveset, moveset.left.coordinates, false);
            if (moveset.left.up != null) {
                moveset.left.up.up = GetUp(ref moveset, moveset.left.up.coordinates, true);

                if (moveset.left.up.up == null) {
                    moveset.left.up = null;
                }
            }

            moveset.left.down = GetDown(ref moveset, moveset.left.coordinates, false);
            if (moveset.left.down != null) {
                moveset.left.down.down = GetDown(ref moveset, moveset.left.down.coordinates, true);

                if (moveset.left.down.down == null) {
                    moveset.left.down = null;
                }
            }

            // Final Check to see if there are any upwards moves whatsoever.
            if (moveset.left.left == null && moveset.left.up == null && moveset.left.down == null) {
                moveset.left = null;
            }
        }







        moveset.right = GetRight(ref moveset, currentCoordinates, false);
        if (moveset.right != null) {
            moveset.right.right = GetRight(ref moveset, moveset.right.coordinates, false);
            if (moveset.right.right != null) {
                moveset.right.right.up = GetUp(ref moveset, moveset.right.right.coordinates, true);
                moveset.right.right.down = GetDown(ref moveset, moveset.right.right.coordinates, true);

                if (moveset.right.right.up == null && moveset.right.right.down == null) {
                    moveset.right.right = null;
                }
            }

            moveset.right.up = GetUp(ref moveset, moveset.right.coordinates, false);
            if (moveset.right.up != null) {
                moveset.right.up.up = GetUp(ref moveset, moveset.right.up.coordinates, true);

                if (moveset.right.up.up == null) {
                    moveset.right.up = null;
                }
            }

            moveset.right.down = GetDown(ref moveset, moveset.right.coordinates, false);
            if (moveset.right.down != null) {
                moveset.right.down.down = GetDown(ref moveset, moveset.right.down.coordinates, true);

                if (moveset.right.down.down == null) {
                    moveset.right.down = null;
                }
            }

            // Final Check to see if there are any upwards moves whatsoever.
            if (moveset.right.right == null && moveset.right.up == null && moveset.right.down == null) {
                moveset.right = null;
            }
        }
    }

    public MoveUp GetUp(ref InitialMove init, IntVector2 prevCoords, bool isEnd) {
        IntVector2 offset = new IntVector2(0, 1);
        IntVector2 newCoords = prevCoords + offset;
        if (parentGame.IsWithinBounds(newCoords)) {
            MoveUp ret = new MoveUp();
            ret.coordinates = newCoords;
            ret.moveOffset = offset;
            ret.reverse = init;
            ret.isPossibleEnd = isEnd;
            init.moveList.Add(ret);
            return ret;
        } else {
            return null;
        }
    }

    public MoveDown GetDown(ref InitialMove init, IntVector2 prevCoords, bool isEnd) {
        IntVector2 offset = new IntVector2(0, -1);
        IntVector2 newCoords = prevCoords + offset;
        if (parentGame.IsWithinBounds(newCoords)) {
            MoveDown ret = new MoveDown();
            ret.coordinates = newCoords;
            ret.moveOffset = offset;
            ret.reverse = init;
            ret.isPossibleEnd = isEnd;
            init.moveList.Add(ret);
            return ret;
        } else {
            return null;
        }
    }

    public MoveLeft GetLeft(ref InitialMove init, IntVector2 prevCoords, bool isEnd) {
        IntVector2 offset = new IntVector2(-1, 0);
        IntVector2 newCoords = prevCoords + offset;
        if (parentGame.IsWithinBounds(newCoords)) {
            MoveLeft ret = new MoveLeft();
            ret.coordinates = newCoords;
            ret.moveOffset = offset;
            ret.reverse = init;
            ret.isPossibleEnd = isEnd;
            init.moveList.Add(ret);
            return ret;
        } else {
            return null;
        }
    }

    public MoveRight GetRight(ref InitialMove init, IntVector2 prevCoords, bool isEnd) {
        IntVector2 offset = new IntVector2(1, 0);
        IntVector2 newCoords = prevCoords + offset;
        if (parentGame.IsWithinBounds(newCoords)) {
            MoveRight ret = new MoveRight();
            ret.coordinates = newCoords;
            ret.moveOffset = offset;
            ret.reverse = init;
            ret.isPossibleEnd = isEnd;
            init.moveList.Add(ret);
            return ret;
        } else {
            return null;
        }
    }

    public IntVector2[] GetSecondaryMoves() {
        List<IntVector2> returnArray = new List<IntVector2>();

        for (int i = 0; i < moveset.moveList.Count; i++) {
            if (!moveset.moveList[i].isPossibleEnd) {
                returnArray.Add(moveset.moveList[i].coordinates);
            }
        }

        return returnArray.ToArray();
    }

    // This is necessarily long.
    public override void MoveTo(IntVector2 coordinates, bool pushed, int distFromPushingPiece, int numPushedPieces) {
        // Determine where it is we're actually moving.
        IntVector2 diff = coordinates - GetCoordinates();

        SetSortingLayer("Moving Pieces");

        //
        // PUSHED
        //

        if (pushed) {
            if (moveDisabled) {
                SetMoveDisabled(false);
            }

            // Get Pushed.
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

            return;
        }

        //
        // MOVED
        //

        if (initialDirection.x != 0) {
            // Move horizontally first, then vertically.
            StartCoroutine(MoveXThenY(diff.x, diff.y, pushed));
            ResetKnight();
        } else if (initialDirection.y != 0) {
            // Move vertically first, then horizontally.
            StartCoroutine(MoveYThenX(diff.x, diff.y, pushed));
            ResetKnight();
        }
    }

    public void SetInitialDirection(IntVector2 dir) {
        initialDirection = dir;
    }

    public bool HasDirection() {
        return initialDirection != IntVector2.NULL;
    }

    public void ResetKnight() {
        initialDirection = IntVector2.NULL;
        currentFingerMove = null;
    }
}