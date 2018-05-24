using System.Collections.Generic;
using Riowil.Entities;
using System.Numerics;

namespace Riowil.Lib
{
    public static class ZVectorBuilder3d
    {
        //For Vector3
        public static IReadOnlyList<ZVector3d> Build(IReadOnlyList<Vector3> points, int[] step, int firstNumber)//firstNumber == series.Points.Count ???
        {
            int n = points.Count;
            int z = step.Length + 1;
            List<ZVector3d> zVectors = new List<ZVector3d>();
            int k;
            //List<Vector3> cur = new List<Vector3>();
            for (int i = 0; i < n; i++)
            {
                k = i;
                List<Vector3> cur = new List<Vector3>();
                for (int j = 0; j < z && k < n; j++)
                {
                    cur.Add(points[k]);
                    k += step[j % step.Length];//???
                }
                if (cur.Count == z)
                {
                    zVectors.Add(new ZVector3d(cur, step, firstNumber++));
                }
            }
            return zVectors;
        }
    }
}
