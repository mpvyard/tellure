using System.Collections.Generic;
using Riowil.Entities;
using System.Numerics;

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
        //For Vector3
        //public static List<ZVector> Build3(List<Vector3> points, int[] step, int firstNumber)//firstNumber == series.Points.Count ???
        //{
        //    int n = points.Count;
        //    int z = step.Length + 1;
        //    List<ZVector> zVectors = new List<ZVector>();
        //    int k;
        //    //List<Vector3> cur = new List<Vector3>();
        //    for (int i = 0; i < n; i++)
        //    {
        //        k = i;
        //        List<Vector3> cur = new List<Vector3>();
        //        for (int j = 0; j < z && k < n; j++)
        //        {
        //            cur.Add(points[k]);
        //            k += step[j % step.Length];//???
        //        }
        //        if (cur.Count == z)
        //        {
        //            zVectors.Add(new ZVector(cur, step, firstNumber++));
        //        }
        //    }
        //    return zVectors;
        //}
    }
}
