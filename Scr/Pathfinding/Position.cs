using RoboticsTools.Util;

namespace RoboticsTools.Pathfinding {
    public struct Position {
        public string label;
        public Vector2Int position;
        public double rotation;

        public int x { get { return position.x; } }
        public int y { get { return position.y; } }

        public Position(string label, Vector2Int position, double rotation) {
            this.label = label;
            this.position = position;
            this.rotation = rotation;
        }

        public void Set(string label, Vector2Int position, double rotation) {
            this.label = label;
            this.position = position;
            this.rotation = rotation;
        }
    }
}