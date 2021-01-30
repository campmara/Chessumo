using UnityEngine;
using System.Collections;

public class Bishop : Piece {
    protected override void Awake() {
        base.Awake();

        moveMagnitude = 10;

        moveOffsets = new IntVector2[4];
        moveOffsets[0] = new IntVector2(-1, 1);
        moveOffsets[1] = new IntVector2(1, 1);
        moveOffsets[2] = new IntVector2(-1, -1);
        moveOffsets[3] = new IntVector2(1, -1);
    }

    public override void DetermineMoveset() {
        moveset = new InitialMove();
        moveset.coordinates = currentCoordinates;

        moveset.up_left = GetUpLeft(ref moveset);
        moveset.up_right = GetUpRight(ref moveset);
        moveset.down_left = GetDownLeft(ref moveset);
        moveset.down_right = GetDownRight(ref moveset);
    }
}