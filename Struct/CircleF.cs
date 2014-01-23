using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace AtlasEngine
{
    public struct CircleF
    {
        public Vector2 Point;
        public float Radius;

        public CircleF(Vector2 position, float radius)
        {
            this.Point = position;
            this.Radius = radius;
        }


        public bool Overlap(Vector2 point)
        {
            return Overlap(this, point, 0);
        }

        public bool Overlap(CircleF circle)
        {
            return Overlap(this, circle.Point, circle.Radius);
        }

        public static bool Overlap(CircleF circle, Vector2 p, float raduis)
        {
            return Vector2.DistanceSquared(circle.Point, p) < (circle.Radius + raduis) * (circle.Radius + raduis);
        }
    }
}
