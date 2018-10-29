using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tellure.Algorithms
{
    public static class DistanceCalculator
    {
        public static float Distance(Vector<float> vector, Vector<float> cluster)
        {
            var count = Vector<double>.Count;
            float dist = 0;
            for (int i = 0; i < count; i++)
            {
                dist += MathF.Pow(cluster[i] - vector[i], 2);
            }
            return MathF.Sqrt(dist);
        }

        public static float Distance(IReadOnlyList<Vector<float>> l1, IReadOnlyList<Vector<float>> l2)
        {
            float sum = 0;

            for (int i = 0; i < l1.Count; i++)
            {
                float dif = Distance(l1[i], l2[i]);
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

        //For Vector3
        //public static float Distance3(IReadOnlyList<Vector3> l1, IReadOnlyList<Vector3> l2)
        //{
        //    float sum = 0;

        //    for (int i = 0; i < l1.Count; i++)
        //    {
        //        float dif = Vector3.Distance(l1[i], l2[i]);
        //        sum += dif * dif;
        //    }

        //    return MathF.Sqrt(sum);
        //}
    }
}
