using RoboticsTools.Util;

namespace RoboticsTools.UI {
    public interface CanvasBrush {
        float value { get; set; }
        int size { get; set; }

        Vector2Int[] GetPixels(Vector2Int selectedPixel);
    }
}