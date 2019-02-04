using System;
using System.Collections.Generic;
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
        public double unit { get; private set; }
        public int resolutionX { get; private set; }
        public int resolutionY { get; private set; }
        public double width { get; private set; }
        public double height { get; private set; }
        public double x = 0;
        public double y = 0;
        private long[,] pixels;

        // Creates a new grid with an x and y resolution;
        public PathfindingGrid(int resolutionX, int resolutionY) {
            if((resolutionX + resolutionY) % 8 != 0) Console.WriteLine("ERR: Resolution must be in powers of 8.");
            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;
            pixels = new long[resolutionX/8, resolutionY/8];

            SetUnitSize(1.0);
        }

        // Set's a 'pixel' (bit) to either on or off.
        public void SetPixel(int x, int y, bool value) {
            int byteX = x / 8;
            int byteY = y / 8;
            int bitX =  x % 8;
            int bitY =  y % 8;

            long bit = (long)1 << (8 * bitX + bitY);

            if (value) pixels[byteX, byteY] |= bit;
		    else pixels[byteX, byteY] &= ~bit;
        }

        // Get the 'pixel' (bit) at the x and y.
        public bool GetPixel(int x, int y) {
            int byteX = x / 8;
            int byteY = y / 8;
            int bitX =  x % 8;
            int bitY =  y % 8;

            long bit = (long)1 << (8 * bitX + bitY);

            Console.WriteLine($"{y}x , {x}y");

            if ((pixels[byteX, byteY] & bit) != 0)
            return true;
		    else return false;
        }

        public bool GetPixel(Vector2Int vector) => GetPixel(vector.x, vector.y);

        // Clears the grid.
        public void Clear() {
            pixels = new long[resolutionX/8, resolutionY/8];
        }

        // Set the resolution.
        public void SetResolution(int resolutionX, int resolutionY) {
            if((resolutionX + resolutionY) % 8 != 0) {
                Console.WriteLine("ERR: Resolution must be in powers of 8.");
                return;
            }
            pixels = new long[resolutionX/8, resolutionY/8];
            this.resolutionX = resolutionX;
            this.resolutionY = resolutionY;
        }

        // Set the unit's per pixel.
        public void SetUnitSize(double centimeters) {
            unit = centimeters;
            unitPerNode = centimeters * PixelToCentimeterRatio;
            width = unitPerNode * resolutionX;
            height = unitPerNode * resolutionY;
        }

        // Get the pixel coords around x and y.
        public Vector2Int[] GetNeighbours(int x, int y) {
            if(IsInBounds(x, y) == false) {
                Console.WriteLine("ERR: Can't get neighbours as requested node is out of range.");
                Console.WriteLine($"X: {x}   Y: {y}");
                return null;
            }

            List<Vector2Int> neighbours = new List<Vector2Int>();
            if(x + 1 < resolutionX)  neighbours.Add(new Vector2Int(x + 1, y));
            if(x - 1 >= 0)           neighbours.Add(new Vector2Int(x - 1, y));
            if(y + 1 < resolutionY)  neighbours.Add(new Vector2Int(x, y + 1));
            if(y - 1 >= 0)           neighbours.Add(new Vector2Int(x, y - 1));

            return neighbours.ToArray();
        }

        // Checks if vector is inside of the grid.
        public bool IsInBounds(Vector2Int pixel) => pixel.x >= 0 && pixel.y >= 0 && pixel.x < resolutionX && pixel.y < resolutionY;
        
        // Checks if x and y are inside of the grid.
        public bool IsInBounds(int x, int y) => x >= 0 && y >= 0 && x < resolutionX && y < resolutionY;

        // Fill a ground of pixels at once.
        public void FillPixels(Vector2Int[] fillPixels, bool value) {
            for(int i = 0; i <= fillPixels.Length-1; i++) {
                if(IsInBounds(fillPixels[i])) SetPixel(fillPixels[i].x, fillPixels[i].y, value);
            }
        }
    }
}