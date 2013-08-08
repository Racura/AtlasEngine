using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace AtlasEngine
{
    public class AtlasColorSystem
    {
        public static Color GetColorFromHue(float hue, float brightness)
        {
            return new Color(GetValueFromHue(hue) * brightness,
                            GetValueFromHue(hue + 120) * brightness,
                            GetValueFromHue(hue + 240) * brightness);
        }

        public static Color GetColorFromHue(float hue)
        {
            return new Color(GetValueFromHue(hue),
                            GetValueFromHue(hue + 120),
                            GetValueFromHue(hue + 240));
        }

        private static float GetValueFromHue(float degree)
        {
            degree = degree % 360;
            if (degree < 0) degree += 360;

            if (degree < 60)  return 1;
            if (degree < 120) return (120 - degree) / 60f;
            if (degree < 240) return 0;
            if (degree < 300) return (degree - 240) / 60f;
            return 1;
        }
    }
}
