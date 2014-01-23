using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace AtlasEngine
{
    public struct LineF
    {
        public Vector2 Start;
        public Vector2 End;


        private Vector2 _start;
        private Vector2 _end;

        private float _dx;
        private float _dy;
        private float _idx;
        private float _idy;

        private void Setup()
        {
            if (_start == Start && _end == End)
                return;

            _start = Start;
            _end = End;

            _dx = End.X - Start.X;
            _dy = End.Y - Start.Y;

            _idx = 1 / _dx;
            _idy = 1 / _dy;
        }

        public float Length()
        {
            return Vector2.Distance(Start, End);
        }
        public float LengthSqrt()
        {
            return Vector2.DistanceSquared(Start, End);
        }


        public bool Overlap(Vector2 point)
        {
            return Overlap(this, point);
        }

        public bool Overlap(CircleF circle)
        {
            return Overlap(this, circle);
        }

        public bool Overlap(RectangleF rectangleF)
        {
            return Overlap(this, rectangleF);
        }

        public bool Overlap(LineF line)
        {
            return Overlap(this, line);
        }



        public static bool Overlap(LineF line, Vector2 p)
        {
            float n1X = line.End.X - line.Start.X;
            float n1Y = line.End.Y - line.Start.Y;
            float n2X = p.X - line.Start.Y;
            float n2Y = p.X - line.Start.Y;

            return Math.Abs(n1Y / n1X - n2Y / n2X) < float.Epsilon
                && Math.Sign(n1X) == Math.Sign(n2X)
                && n2X * n2X + n2Y * n2Y < n1X * n1X + n1Y * n1Y;
        }

        public static bool Overlap(LineF a, LineF b)
        {
            a.Setup();
            b.Setup();

            float s1 = b._dx * (a.Start.Y - b.Start.Y) - b._dy * (a.Start.X - b.Start.X);  
            float s2 = a._dx * (a.Start.Y - b.Start.Y) - a._dy * (a.Start.X - b.Start.X);  
            float d = 1 / (b._dy * a._dx - a._dy * b._dx);

            float u1 = s1 * d;
            float u2 = s2 * d;

            return 0 <= u1 && u1 <= 1
                && 0 <= u2 && u2 <= 1;
        }

        public static bool Overlap(LineF line, CircleF p)
        {
            var apX = p.Point.X - line.Start.X;
            var apY = p.Point.Y - line.Start.Y;
            var abX = line.End.X - line.Start.X;
            var abY = line.End.Y - line.Start.Y;

            var abLength = (abX * abX + abY * abY);

            var dot = apX * abX + apY * abY;

            var t = dot / abLength;

            if (t <= 0)
                return ((apX * apX) + (apY * apY) < p.Radius * p.Radius);
            else if (t >= 1)
                return ((line.End.X - p.Point.X) * (line.End.X - p.Point.X) + (line.End.Y - p.Point.Y) * (line.End.Y - p.Point.Y) < p.Radius * p.Radius);
            else
                return ((apX - abX * t) * (apX - abX * t) + (apY - abY * t) * (apY - abY * t) < p.Radius * p.Radius);
        }

        public static bool Overlap(LineF line, RectangleF rect)
        {
            line.Setup();

            float tmin, tmax, tymin, tymax;

            if (line._dx >= 0)
            {
                tmin = (rect.X - line.Start.X) * line._idx;
                tmax = (rect.X + rect.Width - line.Start.X) * line._idx;
            }
            else
            {
                tmin = (rect.X + rect.Width - line.Start.X) * line._idx;
                tmax = (rect.X - line.Start.X) * line._idx;
            }
            if (line._dy >= 0)
            {
                tymin = (rect.Y - line.Start.Y) * line._idy;
                tymax = (rect.Y + rect.Height - line.Start.Y) * line._idy;
            }
            else
            {
                tymin = (rect.Y + rect.Height - line.Start.Y) * line._idy;
                tymax = (rect.Y - line.Start.Y) * line._idy;
            }
            return !((tmin > tymax) || (tymin > tmax));
        }
    }
}
