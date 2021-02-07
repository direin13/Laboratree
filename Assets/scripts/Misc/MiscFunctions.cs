using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace MiscFunctions
{
    public class NumOp
    {
        public static float Cutoff(float val, float start, float end)
        {
            //used to cutoff numbers going past the ranges start and end
            if (val > end)
                return end;
            else if (val < start)
                return start;
            else
                return val;
        }

        public static int Cutoff(int val, int start, int end)
        {
            if (val > end)
                return end;
            else if (val < start)
                return start;
            else
                return val;
        }

        public static Color GetColorBlend(Color color1, Color color2, float offsetPercentage)
        {
            //Get dist from color1 and color2 
            //and add a pecentage of that onto color1 to get a blend of the 2 colors
            //e.g offset of 0.5f is the halfway color
            offsetPercentage = NumOp.Cutoff(offsetPercentage, 0f, 1f);
            float H1, S1, V1;
            Color.RGBToHSV(color1, out H1, out S1, out V1);

            float H2, S2, V2;
            Color.RGBToHSV(color2, out H2, out S2, out V2);

            Vector3 dist = new Vector3(H2 - H1, S2 - S1, V2 - V1) * offsetPercentage;

            Color out_ = Color.HSVToRGB(H1 + dist[0], S1 + dist[1], V1 + dist[2]);
            return out_;
        }
    }

    public class Parse
    {
        public static Vector2 Vec2(string value)
        {
            //given string in the form "1, 2", will return a vector2 of (1, 2)
            if (value.ElementAt(0) != '(' || value.ElementAt(value.Length-1) != ')')
            {
                throw new ArgumentException(String.Format("Invalid value. Cannot parse string '{0}' to Vector2", value));
            }

            value = value.Substring(1, value.Length - 2);
            string[] nums = value.Split(',');
            try
            {
                return new Vector2(float.Parse(nums[0]), float.Parse(nums[1]));
            }
            catch (System.FormatException e)
            {
                throw new ArgumentException(e.ToString() + String.Format("; Invalid value. Cannot parse string '{0}' to Vector2", value));
            }
        }
    }
}
