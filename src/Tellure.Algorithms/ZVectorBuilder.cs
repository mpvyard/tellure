using System;
using System.Collections.Generic;
using System.Numerics;
using Tellure.Entities;


namespace Tellure.Algorithms
{
	public static class ZVectorBuilder
	{
        public static IReadOnlyList<ZVector> Build(IReadOnlyList<Vector4> points, int[] step, int firstNumber)
        {
            int n = points.Count;
            int z = step.Length + 1;

            var zVectors = new List<ZVector>();
            int k;

            for (int i = 0; i < n; i++)
            {
                k = i;
                var cur = new List<Vector4>();
                for (int j = 0; j < z && k < n; j++)
                {
                    cur.Add(points[k]);
                    k += step[j % step.Length];
                }
                if (cur.Count == z)
                {
                    zVectors.Add(new ZVector(cur, firstNumber++));
                }
            }

            return zVectors;
        }
    }
}
