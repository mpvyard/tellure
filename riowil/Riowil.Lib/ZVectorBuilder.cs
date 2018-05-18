using System.Collections.Generic;
using Riowil.Entities;

namespace Riowil.Lib
{
	public static class ZVectorBuilder
	{

		public static List<ZVector> Build(List<double> points, int[] step, int firstNumber)
		{
			int n = points.Count;
			int z = step.Length+1;
			List<ZVector> zVectors = new List<ZVector>();
			int k;
			for (int i = 0; i < n; i++)
			{
				k = i;
				List<double> cur = new List<double>();
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
