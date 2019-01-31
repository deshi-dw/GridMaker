namespace RoboticsTools.Util {
    public struct Vector2Int {
        public int x;
        public int y;

        public Vector2Int(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x + b.x, a.y + b.y); }
        public static Vector2Int operator -(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x - b.x, a.y - b.y); }
        public static Vector2Int operator *(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x * b.x, a.y * b.y); }
        public static Vector2Int operator /(Vector2Int a, Vector2Int b) { return new Vector2Int(a.x / b.x, a.y / b.y); }

        public static Vector2Int operator *(Vector2Int a, int b) { return new Vector2Int(a.x * b, a.y * b); }
        public static Vector2Int operator /(Vector2Int a, int b) { return new Vector2Int(a.x / b, a.y / b); }

        public static bool operator ==(Vector2Int a, Vector2Int b) => a.x == b.x && a.y == b.y;
        public static bool operator !=(Vector2Int a, Vector2Int b) => a.x != b.x || a.y != b.y;

        public static readonly Vector2Int Zero = new Vector2Int(0, 0);
        public static readonly Vector2Int One = new Vector2Int(1, 1);
    }
}