using UnityEngine;

public class MovePathManager : Singleton<MovePathManager> {
    private Vector2Int startCoords;

    void Awake() {
        startCoords = Constants.I.V2INULL;
    }

    public void BeginPath(Vector2Int coords) {
        startCoords = coords;
    }

    // Done with every piece except the knight.
    public Vector2Int[] CalculatePath(Move move) {
        if (startCoords == Constants.I.V2INULL || move.coordinates == startCoords) return null;

        AudioManager.Instance.PlayPieceDrawMove();

        Vector2Int diff = move.coordinates - startCoords;

        if (diff.x != 0 && diff.y == 0) // horizontal!
        {
            float sign = Mathf.Sign(diff.x);
            int count = Mathf.Abs(diff.x);

            Vector2Int[] ret = new Vector2Int[count];
            for (int i = 0; i < count; i++) {
                ret[i] = new Vector2Int(startCoords.x + (int)((i + 1) * sign), startCoords.y);
            }

            return ret;
        } else if (diff.x == 0 && diff.y != 0) {
            float sign = Mathf.Sign(diff.y);
            int count = Mathf.Abs(diff.y);

            Vector2Int[] ret = new Vector2Int[count];
            for (int i = 0; i < count; i++) {
                ret[i] = new Vector2Int(startCoords.x, startCoords.y + (int)((i + 1) * sign));
            }

            return ret;
        } else if (diff.x != 0 && diff.y != 0) {
            float signX = Mathf.Sign(diff.x);
            float signY = Mathf.Sign(diff.y);
            int count = Mathf.Abs(diff.x);

            Vector2Int[] ret = new Vector2Int[count];
            for (int i = 0; i < count; i++) {
                ret[i] = new Vector2Int(startCoords.x + (int)((i + 1) * signX), startCoords.y + (int)((i + 1) * signY));
            }

            return ret;
        } else {
            return null;
        }
    }

    public Vector2Int[] CalculateKnightPath(Vector2Int[] path, Move move) {
        if (startCoords == Constants.I.V2INULL || move.coordinates == startCoords) return null;

        AudioManager.Instance.PlayPieceDrawMove();

        Vector2Int[] ret = null;

        if (path != null) {
            // if reverse we delete the last item of the path.
            if (move.coordinates == path[path.Length - 1]) {
                ret = new Vector2Int[path.Length - 1];
                for (int i = 0; i < ret.Length; i++) {
                    ret[i] = path[i];
                }
            } else {
                ret = new Vector2Int[path.Length + 1];
                for (int i = 0; i < path.Length; i++) {
                    ret[i] = path[i];
                }

                ret[ret.Length - 1] = new Vector2Int(move.coordinates.x, move.coordinates.y);
            }
        } else {
            ret = new Vector2Int[1];
            ret[0] = new Vector2Int(move.coordinates.x, move.coordinates.y);
        }

        return ret;
    }

    public void EndPath() {
        startCoords = Constants.I.V2INULL;
    }
}
