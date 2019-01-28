using System;

namespace RoboticsTools.Util {
    public static class Vector2IntExtension {
        public static Vector2Int Direction (this Vector2Int a) {
            // return new Vector2Int(a.x/Math.Abs(a.x), a.y/Math.Abs(a.y));
            return new Vector2Int(a.x/-Math.Abs(a.x), a.y/-Math.Abs(a.y)) * -1;
        }

    }
}