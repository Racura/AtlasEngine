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

            hue = (hue % 360) / 60;

            if (hue < 0)
                hue = hue + 6;

            float c = (1 - Math.Abs(light * 2 - 1)) * saturation;
            float x = c * (1 - Math.Abs(hue % 2 - 1));
            float m = light - c * 0.5f;

            const int MOD = 255;

            switch ((int)hue)
            {
                case 0:
                    return new Color(Convert.ToByte((m + c) * MOD), Convert.ToByte((m + x) * MOD), Convert.ToByte((m) * MOD));
                case 1:
                    return new Color(Convert.ToByte((m + x) * MOD), Convert.ToByte((m + c) * MOD), Convert.ToByte((m) * MOD));
                case 2:
                    return new Color(Convert.ToByte((m) * MOD), Convert.ToByte((m + c) * MOD), Convert.ToByte((m + x) * MOD));
                case 3:
                    return new Color(Convert.ToByte((m) * MOD), Convert.ToByte((m + x) * MOD), Convert.ToByte((m + c) * MOD));
                case 4:
                    return new Color(Convert.ToByte((m + x) * MOD), Convert.ToByte((m) * MOD), Convert.ToByte((m + c) * MOD));
                case 5:
                default:
                    return new Color(Convert.ToByte((m + c) * MOD), Convert.ToByte((m) * MOD), Convert.ToByte((m + x) * MOD));
            }
        }
    }
}
