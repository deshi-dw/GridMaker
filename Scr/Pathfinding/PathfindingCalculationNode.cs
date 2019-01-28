using System;

using RoboticsTools;
using RoboticsTools.Util;

namespace RoboticsTools.Pathfinding {
    public class PathfindingCalculationNode {
        public int g;
        public byte f;

        public int x;
        public int y;
        public PathfindingCalculationNode(int g, byte f, int x, int y) {
            this.g = g;
            this.f = f;

            this.x = x;
            this.y = y;
        }

        public PathfindingCalculationNode(int g, byte f, Vector2Int position) {
            this.g = g;
            this.f = f;

            this.x = position.x;
            this.y = position.y;
        }
    }
}