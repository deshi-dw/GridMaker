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
            openSet.Add(new PathfindingCalculationNode(0, grid[start], start));

            PathfindingCalculationNode current = openSet[0];
            Vector2Int[] neighbours = new Vector2Int[4];

            float f;
            int DEBUG_INT = 0;

            while (current.x != goal.x || current.y != goal.y && openSet.Count > 0) {
                Console.WriteLine();
                // Set previous to last current.
                PathfindingCalculationNode previous = current;
                current = openSet[0];
                // f = float.MaxValue;
                
                // Get the lowest 'f' valued node in the open set.
                for (int i = 0; i <= openSet.Count - 1; i++) {
                    if (openSet[i].f < current.f) {
                        current = openSet[i];
                    }
                }
                // Set the new current's previous node to the last current.
                // This will be useful later for constructing the final path.
                current.previous = previous;

                // Remove current from the open set and add it to the closed set.
                openSet.Remove(current);
                closedSet.Add(current);

                // Get the new g of neighbours which is current g + 1 because we are taking another step.
                int g = current.g + 1;

                // Get the new current's neighbours.
                neighbours = grid.GetNeighbours(current.x, current.y);
                Console.WriteLine($"Neighbour Count: {neighbours.Length}");

                // Loop through each neighbour to calculate their f values.
                for (int i = 0; i <= neighbours.Length - 1; i++) {
                    PathfindingCalculationNode neighbour;

                    // If neighbour is part of the closed set, go to next neighbour.
                    if (closedSet.Exists(x => (x.x == neighbours[i].x && x.y == neighbours[i].y))) continue;
                    Console.WriteLine($"Pass: {i+1}/{neighbours.Length}");

                    // If our neighbour is not part of the open set, add them to it.
                    neighbour = openSet.Find(x => (x.x == neighbours[i].x && x.y == neighbours[i].y));
                    if (neighbour == null) {
                        Console.WriteLine($"index {i} : {neighbours[i].x}x {neighbours[i].y}y");
                        neighbour = new PathfindingCalculationNode(g+1, grid[neighbours[i]], neighbours[i]);
                        openSet.Add(neighbour);
                        Console.WriteLine($"{i+1}/{neighbours.Length} Added to openSet.");
                    }

                    Console.WriteLine($"neighbour g: {neighbour.g} : g: {g}");

                    // If the neigbours already have a g value but the path we are on is better then choose our path's g.
                    if (g <= neighbour.g) {
                        neighbour.g = g;

                        // Since our path seems better then the one we are overriding, we need to recalculate it's f value.
                        // Calculating h is the distance between the goal and the neigbour. To do this we need (vector2Int a - vector2Int b).magnitude.
                        // Magnitude is calculated as the square root of x*x + y*y.
                        int tempx = neighbour.x - goal.x;
                        int tempy = neighbour.y - goal.y;
                        double h = Math.Sqrt(tempx * tempx + tempy * tempy);
                        // the f value is g + h. We convert it into a float to saving memory.
                        grid[neighbour.x, neighbour.y] = (float)(h + g);

                        neighbour.f = grid[neighbour.x, neighbour.y];
                        Console.WriteLine($"h: {h}");
                    }

                }
                // if(DEBUG_INT == 30) return;
                DEBUG_INT++;
            }

            // Now construct the path.
            while(current.x != start.x || current.y != start.y) {
                path.Enqueue(new Vector2Int(current.x, current.y));
                current = current.previous;
            }
        }
    }
}