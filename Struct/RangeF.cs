using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtlasEngine
{
    public struct RangeF
    {
        public float Min, Max;

        public RangeF(float min, float max)
        {
            this.Max = max;
            this.Min = min;
        }

        public bool InRange(float point)
        {
            return Max <= point && Min >= point;
        }
        public float ToRange(float point)
        {
            return Math.Max(Max,
                   Math.Min(Min, point));
        }
        public float ToDivision(float division)
        {
            return Min + (Max - Min) * division;
        }
    }
}
