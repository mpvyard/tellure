using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Riowil.Lib
{
    public static class MathExtended
    {
        public static double MAX_DIV = 0.05;
        public static double Distance(IReadOnlyList<float> l1, IReadOnlyList<float> l2)
        {
            double sum = 0;

            for (int i = 0; i < l1.Count; i++)
            {
                double dif = l1[i] - l2[i];
                sum += dif * dif;
            }
            return Math.Sqrt(sum);
        }

        public static double AbsDivergence(double value1, double value2)
        {
            double dif = value1 - value2;
            return Math.Abs(dif);
        }

        public static double SqrtDivergence(double value1, double value2)
        {
            double dif = value1 - value2;
            return dif * dif;
        }

        //For Vector3
        public static double Distance3(IReadOnlyList<Vector3> l1, IReadOnlyList<Vector3> l2)
        {
            double sum = 0;

            for (int i = 0; i < l1.Count; i++)
            {
                double dif = Vector3.Distance(l1[i],l2[i]);
                sum += dif * dif;
            }

            return Math.Sqrt(sum);
        }
    }
}
