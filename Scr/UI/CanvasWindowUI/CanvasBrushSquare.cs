using System;
using RoboticsTools.Util;

namespace RoboticsTools.UI {
    public struct CanvasBrushSquare : CanvasBrush {
        public byte value { get; set; }
        public int size { get; set; }

        public CanvasBrushSquare(byte opacity, int size) {
            this.value = opacity;
            this.size = size;
        }

        public Vector2Int[] GetPixels(Vector2Int selectedPixel) {
            int width = size*2+1;
            Vector2Int[] pixels = new Vector2Int[width*width];
            for(int x = selectedPixel.x-size; x <= selectedPixel.x; x++) {
                for(int y = selectedPixel.y-size; y <= selectedPixel.y; y++) {
                    int index = (x - selectedPixel.x+size) * width + (y - selectedPixel.y+size);
                    // Console.WriteLine($"index {index}     Actual(x:{x} y:{y})     Adjusted(x:{(x - selectedPixel.x+size)} y:{(y - selectedPixel.y+size)})");
                    pixels[index] = new Vector2Int(x, y);
                }
            }
            return pixels;
        }
    }
}