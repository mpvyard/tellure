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

        public static double Distance(List<double> l1, List<double> l2)
        {
            double sum = 0;

            for (int i = 0; i < l1.Count; i++)
            {
                double dif = l1[i] - l2[i];
                sum += dif * dif;
            }
            return Math.Sqrt(sum);
        }

        public static double Distance(double[] currentVector, double[] realVector)
        {
            double sum = 0;

            for (int i = 0; i < currentVector.Length; i++)
            {
                double dif = currentVector[i] - realVector[i];
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

        public static List<double> Normilize(List<double> vector)
        {
            double normParam = Math.Abs(vector.Min()) + Math.Abs(vector.Max());
            return vector.Select(el => el / normParam).ToList();
        }

        public static List<double> NormilizeN(List<double> vector)
        {
            double normParam = vector.Max() - vector.Min();
            double min = vector.Min();
            return vector.Select(el => (el - min) / normParam).ToList();
        }
        //For Vector3
        public static double Distance3(List<Vector3> l1, List<Vector3> l2)
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
