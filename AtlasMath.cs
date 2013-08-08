using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace AtlasEngine
{
    public class AtlasMath
    {
        public static float DistanceFromLineSquared(Vector2 v, Vector2 w, Vector2 point, bool isSegment)
        {
            // Return minimum distance between line segment vw and point p
            float l2 = Vector2.DistanceSquared(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) return Vector2.DistanceSquared(point, v);   // v == w case
            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            float t = Vector2.Dot(point - v, w - v) / l2;
            if (isSegment)
            {
                if (t < 0.0) return Vector2.DistanceSquared(point, v);       // Beyond the 'v' end of the segment
                else if (t > 1.0) return Vector2.DistanceSquared(point, w);  // Beyond the 'w' end of the segment
            }
            Vector2 projection = v + t * (w - v);  // Projection falls on the segment
            return Vector2.DistanceSquared(point, projection);
        }

        public static float DistanceFromLine(Vector2 v, Vector2 w, Vector2 point, bool isSegment)
        {
            // Return minimum distance between line segment vw and point p
            float l2 = Vector2.DistanceSquared(v, w);  // i.e. |w-v|^2 -  avoid a sqrt
            if (l2 == 0.0) return Vector2.Distance(point, v);   // v == w case
            // Consider the line extending the segment, parameterized as v + t (w - v).
            // We find projection of point p onto the line. 
            // It falls where t = [(p-v) . (w-v)] / |w-v|^2
            float t = Vector2.Dot(point - v, w - v) / l2;
            if (isSegment)
            {
                if (t < 0.0) return Vector2.Distance(point, v);       // Beyond the 'v' end of the segment
                else if (t > 1.0) return Vector2.Distance(point, w);  // Beyond the 'w' end of the segment
            }
            Vector2 projection = v + t * (w - v);  // Projection falls on the segment
            return Vector2.Distance(point, projection);
        }
    }
}
