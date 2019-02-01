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
        public Queue<Vector2Int> path;
        public Pathfinding(PathfindingGrid grid) {
            openSet = new List<PathfindingCalculationNode>();
            closedSet = new List<PathfindingCalculationNode>();

            this.grid = grid;
            this.path = new Queue<Vector2Int>();
        }

        public void Solve(Vector2Int start, Vector2Int goal) {
            openSet = new List<PathfindingCalculationNode>();
            closedSet = new List<PathfindingCalculationNode>();
            openSet.Add(new PathfindingCalculationNode(0, grid[start], start));
            PathfindingCalculationNode current = openSet[0];
            PathfindingCalculationNode previous;
            Vector2Int[] neighbours = new Vector2Int[4];

            while (true) {
                // Set previous to last current.
                previous = current;
                if(openSet.Count <= 0) break;
                current = openSet[0];

                // Get the lowest 'f' valued node in the open set.
                for (int i = 0; i < openSet.Count - 1; i++) {
                    if (openSet[i].f <= current.f) {
                        current = openSet[i];
                    }
                }

                if (current.x == goal.x-1 && current.y == goal.y-1) break;

                // Remove current from the open set and add it to the closed set.
                openSet.Remove(current);
                closedSet.Add(current);

                // Get the new current's neighbours.
                neighbours = grid.GetNeighbours(current.x, current.y);
                int g = current.g + 1;

                // Loop through each neighbour to calculate their f values.
                for (int i = 0; i <= neighbours.Length - 1; i++) {
                    PathfindingCalculationNode neighbour;

                    if (grid[neighbours[i]] < 0) continue;
                    // If neighbour is part of the closed set, go to next neighbour.
                    if (closedSet.Exists(node => (node.x == neighbours[i].x && node.y == neighbours[i].y))) continue;

                    // If our neighbour is not part of the open set, add them to it.
                    neighbour = openSet.Find(node => (node.x == neighbours[i].x && node.y == neighbours[i].y));
                    if (neighbour == null) {
                        neighbour = new PathfindingCalculationNode(g + 1, grid[neighbours[i]], neighbours[i]);
                        openSet.Add(neighbour);
                    }

                    // If the neigbours already have a g value but the path we are on is better then choose our path's g.
                    if (g < neighbour.g) {
                        neighbour.g = g;

                        // Since our path seems better then the one we are overriding, we need to recalculate it's f value.
                        // Calculating h is the distance between the goal and the neigbour. c^2 = a^2 + b^2
                        int tempx = neighbour.x - goal.x;
                        int tempy = neighbour.y - goal.y;
                        double h = Math.Sqrt(tempx * tempx + tempy * tempy);
                        // the f value is g + h. We convert it into a float to saving memory.
                        grid[neighbour.x, neighbour.y] = (float) (h + g * 0.5);

                        neighbour.f = grid[neighbour.x, neighbour.y];
                        // Set the neighbours previous to current.
                        // This will be useful later for constructing the final path.
                        neighbour.previous = current;
                    }

                }

                path = new Queue<Vector2Int>();
            }

            // Now construct the path.
            while (current.x != start.x || current.y != start.y) {
                path.Enqueue(new Vector2Int(current.x, current.y));
                current = current.previous;
            }
        }
    }
}