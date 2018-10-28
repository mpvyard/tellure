using System;
using System.Collections.Generic;
using System.Numerics;
using Tellure.Entities;

namespace Tellure.Algorithms
{
    public static class ZVectorBuilder3d
    {
        //For Vector3
        [Obsolete]
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

        public static IReadOnlyList<Vector<Vector3>> BuildVector(IReadOnlyList<Vector3> points, int[] step, int firstNumber)
        {
            int n = points.Count;
            int z = step.Length + 1;

            var zVectors = new List<Vector<Vector3>>();
            int k;


            for (int i = 0; i < n; i++)
            {
                k = i;
                var cur = new List<Vector3>();
                for (int j = 0; j < z && k < n; j++)
                {
                    cur.Add(points[k]);
                    k += step[j % step.Length];
                }
                if (cur.Count == z)
                {
                    zVectors.Add(new Vector<Vector3>(cur.ToArray()));
                }
            }

            return zVectors;
        }
    }
}
