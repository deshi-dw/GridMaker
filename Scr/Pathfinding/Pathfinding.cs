using System;
using System.Collections.Generic;
using RoboticsTools.Util;

namespace RoboticsTools.Pathfinding {
    public class Pathfinding {
        // TODO: Add Comments.
        private List<PathfindingCalculationNode> openSet;
        private List<PathfindingCalculationNode> closedSet;

        private PathfindingGrid grid;

        // TODO: Add path generation.
        public Vector2Int[] path;
        public Pathfinding(PathfindingGrid grid) {
            openSet = new List<PathfindingCalculationNode>();
            closedSet = new List<PathfindingCalculationNode>();

            this.grid = grid;
        }

        public void Solve(Vector2Int start, Vector2Int goal) {
            openSet.Add(new PathfindingCalculationNode(0, grid[start], start));

            PathfindingCalculationNode current = openSet[0];
            Vector2Int[] neighbours = new Vector2Int[4];

            double f;

            while (current.x != goal.x && current.y != goal.y) {
                for (int i = 0; i <= openSet.Count - 1; i++) {
                    // Get the lowest 'f' valued node in the open set.
                    f = 1.0 / current.f;
                    if (openSet[i].f < f) {
                        current = openSet[i];
                    }
                }

                // Remove current from the open set and add it to the closed set.
                openSet.Remove(current);
                closedSet.Add(current);

                // Get the new g of neighbours which is current g + 1 because we are taking another step.
                int g = current.g + 1;

                // Get the new current's neighbours.
                neighbours = grid.GetNeighbours(current.x, current.y);

                for (int j = 0; j <= neighbours.Length - 1; j++) {
                    PathfindingCalculationNode neighbour;

                    // If neighbour is part of the closed set, continue.
                    if (closedSet.Exists(x => (x.x == neighbours[j].x && x.y == neighbours[j].y))) continue;

                    // If our neighbour is not part of the open set, add them to it.
                    neighbour = openSet.Find(x => (x.x == neighbours[j].x && x.y == neighbours[j].y));
                    if (neighbour == null) {
                        neighbour = new PathfindingCalculationNode(g, grid[neighbours[j]], neighbours[j]);
                        openSet.Add(neighbour);
                    }

                    // If the neigbours already have a g value but the path we are on is better then choose our path's g.
                    if (g < neighbour.g) {
                        neighbour.g = g;

                        // Since our path seems better then the one we are overriding, we need to recalculate it's f value.
                        // Calculating h is the distance between the goal and the neigbour. To do this we need (vector2Int a - vector2Int b).magnitude.
                        // Magnitude is calculated as the square root of x*x + y*y.
                        int tempx = neighbours[j].x - goal.x;
                        int tempy = neighbours[j].y - goal.y;
                        double h = Math.Sqrt(tempx * tempx + tempy * tempy);
                        grid[neighbours[j]] = (byte) (h + g);
                    }

                }
            }
        }
    }
}