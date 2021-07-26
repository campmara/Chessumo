[System.Serializable]
public struct IntVector2 {
    public int x, y;
    public static IntVector2 NULL { get { return new IntVector2(-999999999, -999999999); } }

    public IntVector2(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(IntVector2 a, IntVector2 b) {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(IntVector2 a, IntVector2 b) {
        return a.x != b.x || a.y != b.y;
    }

    public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
        a.x += b.x;
        a.y += b.y;
        return a;
    }

    public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
        a.x -= b.x;
        a.y -= b.y;
        return a;
    }

    public static IntVector2 operator *(IntVector2 a, int b) {
        a.x *= b;
        a.y *= b;
        return a;
    }

    public override bool Equals(System.Object obj) {
        // If parameter is null return false.
        if (obj == null) {
            return false;
        }

        // If parameter cannot be cast to IntVector2 return false.
        IntVector2 v = (IntVector2)obj;
        if ((System.Object)v == null) {
            return false;
        }

        // Return true if the fields match:
        return (x == v.x) && (y == v.y);
    }

    public bool Equals(IntVector2 b) {
        return x == b.x && y == b.y;
    }

    public override int GetHashCode() {
        return x ^ y;
    }
}
