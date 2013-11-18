using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AtlasEngine
{
    public struct RectangleF : IEquatable<RectangleF>
    {
        public float    X,
                        Y,
                        Width,
                        Height;

        public Vector2 Center
        {
            get
            {
                return new Vector2(X + Width / 2, Y + Height / 2);
            }
        }


        public Rectangle Bounds { get { return new Rectangle(0,0,(int)Width, (int)Height); } }

        public Vector2 Position { get { return new Vector2(X, Y); } }


        public RectangleF(float width, float height)
        {
            this.X = this.Y = 0;
            this.Width = width;
            this.Height = height;
        }

        public RectangleF(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Overlap(RectangleF other)
        {
            return Overlap(this, other);
        }

        public bool Overlap(Vector2 corner1, Vector2 corner2)
        {
            return Overlap(this, corner1, corner2);
        }

        public static bool Overlap(RectangleF rect1, RectangleF rect2)
        {
            return !((rect1.X > rect2.X + rect2.Width)
                || (rect2.X > rect1.X + rect1.Width)
                || (rect1.Y > rect2.Y + rect2.Height)
                || (rect2.Y > rect1.Y + rect1.Height));
        }

        public static bool Overlap(RectangleF rect, Vector2 corner1, Vector2 corner2)
        {
            return !((rect.X > Math.Max(corner1.X, corner2.X))
                || (Math.Min(corner1.X, corner2.X) > rect.X + rect.Width)
                || (rect.Y > Math.Max(corner1.Y, corner2.Y))
                || (Math.Min(corner1.Y, corner2.Y) > rect.Y + rect.Height));
        }

        public bool Overlap(Vector2 v)
        {
            return Overlap(this, v);
        }

        public static bool Overlap(RectangleF rect1, Vector2 v)
        {
            return !((rect1.X > v.X)
                || (v.X > rect1.X + rect1.Width)
                || (rect1.Y > v.Y)
                || (v.Y > rect1.Y + rect1.Height));
        }

        public bool Equals(RectangleF other)
        {
            return X == other.X
                && Y == other.Y
                && Width == other.Width
                && Height == other.Height;
        }

        public RectangleF Move(Vector2 position)
        {
            return new RectangleF(X + position.X, Y + position.Y, Width, Height);
        }
    }
}
