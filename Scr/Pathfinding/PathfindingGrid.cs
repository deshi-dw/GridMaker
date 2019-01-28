using System;
using System.Threading;
using System.Threading.Tasks;

using RoboticsTools;
using RoboticsTools.Util;

namespace RoboticsTools.Pathfinding {
    public class PathfindingGrid {
        // FIXME: Move some of these variables to another class or something...
        // TODO: Add Comments.
        public static readonly double PixelToCentimeterRatio = 1.411663807890223;
        public double unitPerNode { get; private set; }
        public int resolutionX { get; private set; }
        public int resolutionY { get; private set; }
        public double width { get; private set; }
        public double height { get; private set; }
        public double x = 0;
        public double y = 0;
        private byte[,] nodes;

        public PathfindingGrid(int width, int height) {
            this.resolutionX = width;
            this.resolutionY = height;
            nodes = new byte[width, height];

            SetCmPerNode(1.0);
        }

        public byte this[int x, int y] { get{ return nodes[x, y]; } set{ nodes[x, y] = value; } }
        public byte this[Vector2Int coord] { get{ return nodes[coord.x, coord.y]; } set{ nodes[coord.x, coord.y] = value; } }

        public void Clear() {
            nodes = new byte[resolutionX, resolutionY];
        }

        public void SetResolution(int resolutionX, int resolutionY) {
            byte[,] newNodes = new byte[resolutionX, resolutionY];
            for(int x = 0; x <= newNodes.GetLength(0)-1; x++) {
                for(int y = 0; y <= newNodes.GetLength(1)-1; y++) {
                    // if(x <= nodes.Length-1 && y <= nodes.GetLength(1)-1) newNodes[x, y] = nodes[x, y];
                    // else {
                    // Console.WriteLine($"{x}x, {y}y");
                    newNodes[x, y] = 0;
                    // }
                }
            }

            nodes = newNodes;
            this.resolutionX = newNodes.GetLength(0);
            this.resolutionY = newNodes.GetLength(1);
            return;
        }

        public void SetCmPerNode(double centimeters) {
            unitPerNode = centimeters * PixelToCentimeterRatio;
            width = unitPerNode * resolutionX;
            height = unitPerNode * resolutionY;
        }

        public Vector2Int[] GetNeighbours(int x, int y) {
            if(x > nodes.Length-1 || x < nodes.Length-1 || y > nodes.GetLength(x)-1 || y < nodes.GetLength(x)-1) {
                Console.WriteLine("ERR: Can't get neighbours as requested node is out of range.");
                return null;
            }

            Vector2Int[] neighbours = new Vector2Int[4];
            if(x + 1 < nodes.Length-1) neighbours[1] = new Vector2Int(x + 1, y);
            if(x - 1 < nodes.Length-1) neighbours[2] = new Vector2Int(x + 1, y);
            if(y + 1 < nodes.GetLength(x)-1) neighbours[3] = new Vector2Int(x, y + 1);
            if(y - 1 < nodes.GetLength(x)-1) neighbours[4] = new Vector2Int(x, y - 1);

        return neighbours;
        }

        public void FillRect(Vector2Int start, Vector2Int end, byte value) {
            Vector2Int direction = (start - end).Direction();

            for(int x = start.x; x != end.x-direction.x; x -= direction.x) {
                for(int y = start.y; y != end.y-direction.y; y -= direction.y) {
                    nodes[x, y] = value;
                }
            }
        }
    }
}