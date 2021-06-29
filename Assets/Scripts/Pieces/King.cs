public class King : Piece {
    protected override void Awake() {
        base.Awake();

        moveMagnitude = 1;

        moveOffsets = new IntVector2[8];
        moveOffsets[0] = new IntVector2(-1, 1);
        moveOffsets[1] = new IntVector2(0, 1);
        moveOffsets[2] = new IntVector2(1, 1);

        moveOffsets[3] = new IntVector2(-1, 0);
        moveOffsets[4] = new IntVector2(1, 0);

        moveOffsets[5] = new IntVector2(-1, -1);
        moveOffsets[6] = new IntVector2(0, -1);
        moveOffsets[7] = new IntVector2(1, -1);
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
