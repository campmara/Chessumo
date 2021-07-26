using UnityEngine;

public class Queen : Piece {
    protected override void Awake() {
        base.Awake();

        pieceID = 1;

        moveMagnitude = 10;

        moveOffsets = new Vector2Int[8];
        moveOffsets[0] = new Vector2Int(-1, 1);
        moveOffsets[1] = new Vector2Int(0, 1);
        moveOffsets[2] = new Vector2Int(1, 1);

        moveOffsets[3] = new Vector2Int(-1, 0);
        moveOffsets[4] = new Vector2Int(1, 0);

        moveOffsets[5] = new Vector2Int(-1, -1);
        moveOffsets[6] = new Vector2Int(0, -1);
        moveOffsets[7] = new Vector2Int(1, -1);
    }

    public override void DetermineMoveset() {
        moveset = new InitialMove();
        moveset.coordinates = currentCoordinates;

        moveset.up = GetUp(ref moveset);
        moveset.down = GetDown(ref moveset);
        moveset.left = GetLeft(ref moveset);
        moveset.right = GetRight(ref moveset);

        moveset.up_left = GetUpLeft(ref moveset);
        moveset.up_right = GetUpRight(ref moveset);
        moveset.down_left = GetDownLeft(ref moveset);
        moveset.down_right = GetDownRight(ref moveset);
    }
}
