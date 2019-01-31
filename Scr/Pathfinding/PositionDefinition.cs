using RoboticsTools.Util;

namespace RoboticsTools.Pathfinding {
    public struct PositionDefinition {
        public string label;
        public int id;
        public Vector2Int position;

        public PositionDefinition(string label, int id, Vector2Int position) {
            this.label = label;
            this.id = id;
            this.position = position;
        }

        public void SetPosition(int x, int y) {
            position.x = x;
            position.y = y;
        }
    }
}