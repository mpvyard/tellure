using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tellure.Algorithms
{
    public static class DistanceCalculator
    {
        public static float Distance(IReadOnlyList<Vector4> l1, IReadOnlyList<Vector4> l2)
        {
            float sum = 0;

            for (int i = 0; i < l1.Count; i++)
            {
                float dif = Vector4.Distance(l1[i], l2[i]);
                sum += dif * dif;
            }
            return MathF.Sqrt(sum);
        }

        public static float Distance(IReadOnlyList<float> l1, IReadOnlyList<float> l2)
        {
            float sum = 0;

            for (int i = 0; i < l1.Count; i++)
            {
                float dif = l1[i] - l2[i];
                sum += dif * dif;
            }
            return MathF.Sqrt(sum);
        }
    }
}
