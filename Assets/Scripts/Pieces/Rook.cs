﻿using UnityEngine;

public class Rook : Piece {
    protected override void Awake() {
        base.Awake();

        // This piece can move pretty much across the board;
        moveMagnitude = 10;

        moveOffsets = new Vector2Int[4];

        moveOffsets[0] = new Vector2Int(0, 1);
        moveOffsets[1] = new Vector2Int(0, -1);
        moveOffsets[2] = new Vector2Int(1, 0);
        moveOffsets[3] = new Vector2Int(-1, 0);
    }

    public override void DetermineMoveset() {
        moveset = new InitialMove();
        moveset.coordinates = currentCoordinates;

        moveset.up = GetUp(ref moveset);
        moveset.down = GetDown(ref moveset);
        moveset.left = GetLeft(ref moveset);
        moveset.right = GetRight(ref moveset);
    }
}













