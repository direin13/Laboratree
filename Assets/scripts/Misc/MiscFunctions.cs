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
    }

    public class Parse
    {
        public static Vector2 Vec2(string value)
        {
            //given string in the form "1, 2", will return a vector2 of (1, 2)
            string[] nums = value.Split(',');
            return new Vector2(float.Parse(nums[0]), float.Parse(nums[1]));
        }
    }
}
