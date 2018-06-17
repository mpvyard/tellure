using System.Collections.Generic;
using Tellure.Entities;

namespace Tellure.Algorithms
{
	public static class ZVectorBuilder
	{

		public static IReadOnlyList<ZVector> Build(IReadOnlyList<float> points, int[] step, int firstNumber)
		{
			int n = points.Count;
			int z = step.Length+1;
            var zVectors = new List<ZVector>();
			int k;
			for (int i = 0; i < n; i++)
			{
				k = i;
				List<float> cur = new List<float>();
				for (int j = 0; j < z && k < n; j++)
				{
					cur.Add(points[k]);
					k += step[j % step.Length];
				}
				if (cur.Count == z)
				{
					zVectors.Add(new ZVector(cur, step, firstNumber++));
				}
			}
			return zVectors;
		}
    }
}
