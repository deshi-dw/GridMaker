using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace RoboticsTools.UI {
    public struct SmallPixel : IPixel<SmallPixel>, IPixel {
        public byte alpha;
        public byte color;

        public SmallPixel(byte color, byte alpha) {
            this.color = color;
            this.alpha = alpha;
        }

        public SmallPixel(byte r, byte g, byte b, byte a) {
            // Console.WriteLine($"GetByte = {Convert.ToString(GetRgb8(r, g, b), 2)}");
            color = (byte)GetRgb8(r, g, b);
            alpha = a;
        }

        public static byte GetRgb8(byte r, byte g, byte b) {
            return (byte)((r & 0xE0) | ((g & 0xE0)>>3) | (b >> 6));
        }

        public int GetColor() {; 
            // Console.WriteLine($"{R}red : {G}green : {B}blue");
            return alpha << 24 | ((color & 0xE0) >> 5)*200 << 16 | ((color & 0x1C) >> 2)*200 << 8 | (color & 0x03)*200;
        }

        public PixelOperations<SmallPixel> CreatePixelOperations() => new PixelOperations<SmallPixel>();

        public bool Equals(SmallPixel other) => other.alpha == this.alpha && other.color == this.color ? true : false;

        public void PackFromArgb32(Argb32 source) {
            color = (byte)GetRgb8(source.R, source.G, source.B);
            alpha = source.A;
        }
        public void PackFromBgra32(Bgra32 source) {
            color = (byte)GetRgb8(source.R, source.G, source.B);
            alpha = source.A;
        }
        public void PackFromRgb48(Rgb48 source) {
            Argb32 temp = new Argb32();
            source.ToArgb32(ref temp);
            PackFromArgb32(temp);
        }
        public void PackFromRgba32(Rgba32 source) {
            color = GetRgb8(source.R, source.G, source.B);
            alpha = source.A;
            // Console.WriteLine($"RED={(color & 0xE0) >> 5} : GREEN{(color & 0x1C) >> 2} : BLUE{color & 0x03}");
        }
        public void PackFromRgba64(Rgba64 source) {
            Argb32 temp = new Argb32();
            source.ToArgb32(ref temp);
            PackFromArgb32(temp);
        }
        public void PackFromScaledVector4(Vector4 vector) {
            color = (byte)GetRgb8((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
            alpha = (byte)vector.W;
        }
        public void PackFromVector4(Vector4 vector) {
            color = (byte)GetRgb8((byte)vector.X, (byte)vector.Y, (byte)vector.Z);
            alpha = (byte)vector.W;
        }
        public void ToArgb32(ref Argb32 dest) {
            dest.R = (byte)((color & 0xE0) >> 5);
            dest.G = (byte)((color & 0x1C));
            dest.B = (byte)(color & 0x03);
            dest.A = alpha;
        }
        public void ToBgr24(ref Bgr24 dest) {
            dest.R = (byte)((color & 0xE0) >> 5);
            dest.G = (byte)((color & 0x1C));
            dest.B = (byte)(color & 0x03);
        }
        public void ToBgra32(ref Bgra32 dest) {
            dest.R = (byte)((color & 0xE0) >> 5);
            dest.G = (byte)((color & 0x1C));
            dest.B = (byte)(color & 0x03);
            dest.A = alpha;
        }
        public void ToRgb24(ref Rgb24 dest) {
            dest.R = (byte)((color & 0xE0) >> 5);
            dest.G = (byte)((color & 0x1C));
            dest.B = (byte)(color & 0x03);
        }
        public void ToRgb48(ref Rgb48 dest) {
            Argb32 temp = new Argb32();
            temp.R = (byte)((color & 0xE0) >> 5);
            temp.G = (byte)((color & 0x1C));
            temp.B = (byte)(color & 0x03);
            temp.A = alpha;
            dest.PackFromArgb32(temp);
        }
        public void ToRgba32(ref Rgba32 dest) {
            dest.R = (byte)((color & 0xE0) >> 5);
            dest.G = (byte)((color & 0x1C));
            dest.B = (byte)(color & 0x03);
            dest.A = alpha;
        }
        public void ToRgba64(ref Rgba64 dest) {
            Argb32 temp = new Argb32();
            temp.R = (byte)((color & 0xE0) >> 5);
            temp.G = (byte)((color & 0x1C));
            temp.B = (byte)(color & 0x03);
            temp.A = alpha;
            dest.PackFromArgb32(temp);
        }
        public Vector4 ToScaledVector4() {
            return new Vector4();
        }
        public Vector4 ToVector4() {
            return new Vector4();
        }

        public static bool operator ==(SmallPixel left, SmallPixel right) => left.alpha == right.alpha && left.color == right.color ? true : false;
        public static bool operator !=(SmallPixel left, SmallPixel right) => left.alpha != right.alpha && left.color != right.color ? true : false;
    }
}