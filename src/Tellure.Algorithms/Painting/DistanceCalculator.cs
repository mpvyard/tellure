using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Tellure.Algorithms.Painting
{
    public static class DistanceCalculator
    {
        public static double Distance(float[] vector, float[] cluster)
        {
            var count = vector.Length;
            double dist = 0;
            for (int i = 0; i < count; i++)
            {
                dist += Math.Pow(cluster[i] - vector[i], 2);
            }
            return Math.Sqrt(dist);
        }

        public static double Distance(Vector<double> vector, Vector<double> cluster)
        {
            var count = Vector<double>.Count;
            double dist = 0;
            for (int i = 0; i < count; i++)
            {
                dist += Math.Pow(cluster[i] - vector[i], 2);
            }
            return Math.Sqrt(dist);
        }
    }
}
