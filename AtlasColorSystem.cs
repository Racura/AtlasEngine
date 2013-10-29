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
        public static Color RGBFromHSL(float hue, float saturation, float light)
        {
            if (saturation <= 0)
                return new Color(light, light, light);

            hue = ((hue + (hue < 0 ? 6 : 0)) % 360) / 60;

            if (hue < 0)
                hue = hue + 6;

            float c = (1 - Math.Abs(light * 2 - 1)) * saturation;
            float x = c * (1 - Math.Abs(hue % 2 - 1));
            float m = light - c * 0.5f;

            switch ((int)hue)
            {
                case 0:
                    return new Color(m + c, m + x, m);
                case 1:
                    return new Color(m + x, m + c, m);
                case 2:
                    return new Color(m, m + c, m + x);
                case 3:
                    return new Color(m, m + x, m + c);
                case 4:
                    return new Color(m + x, m, m + c);
                case 5:
                default:
                    return new Color(m + c, m, m + x);
            }
        }
    }
}
