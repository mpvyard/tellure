using System;
using System.Collections.Generic;
using System.Numerics;

namespace Tellure.Algorithms
{
    public static class MathExtended
    {
        public static double MAX_DIV = 0.05;

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

        public static (double, int) CalculateRMSE(IList<float> result, IList<float> sequence)
        {
            int count = 0, nonpred = 0;
            double mse = 0;
            for (int i = 1000; i < result.Count; i++)
            {
                if (!Single.IsNaN(result[i]))
                {
                    mse += Math.Pow(result[i] - sequence[i], 2);
                    count++;
                }
                else
                {
                    nonpred++;
                }
            }
            mse = mse / count;
            double rmse = Math.Sqrt(mse);
            return (rmse, nonpred);
        }
    }
}
