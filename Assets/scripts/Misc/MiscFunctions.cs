using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
