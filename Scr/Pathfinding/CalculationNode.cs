using System;

using RoboticsTools;
using RoboticsTools.Util;

namespace RoboticsTools.Pathfinding {
    public class CalculationNode {
        public int g;
        public float f;

        public int x;
        public int y;

        public CalculationNode previous;
        public CalculationNode(int g, float f, int x, int y) {
            this.g = g;
            this.f = f;

            this.x = x;
            this.y = y;
        }

        public CalculationNode(int g, float f, Vector2Int position) {
            this.g = g;
            this.f = f;

            this.x = position.x;
            this.y = position.y;
        }
    }
}