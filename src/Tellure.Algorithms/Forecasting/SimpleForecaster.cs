using System;
using System.Collections.Generic;
using System.Text;

namespace Tellure.Algorithms.Forecasting
{
    public class SimpleForecaster
    {
        public static IList<float> Forecast(IList<Template> templates,
            IList<float[][]> clusters,
            IList<float> sequence,
            float error)
        {
            var result = new float[sequence.Count];
            var clustersCount = new int[sequence.Count];
            int multiplier = 1;// Convert.ToInt32(textBox7.Text);

            for (int i = 0; i < 1000; i++)
            {
                result[i] = sequence[i];
            }

            int a1 = 1, b1 = 1, c1 = 1, d1 = 1;
            int a2 = 10, b2 = 10, c2 = 10, d2 = 10;

            for (int i = 1000; i < sequence.Count; i++)
            {
                float currDistance = Single.MaxValue;
                float currResult = Single.NaN;

                for (int a = a1; a <= a2; a++)
                {
                    for (int b = b1; b <= b2; b++)
                    {
                        for (int c = c1; c <= c2; c++)
                        {
                            for (int d = d1; d <= d2; d++)
                            {
                                var vector = new float[4];

                                vector[0] = sequence[i - Math.Abs((-d - c - b - a) * multiplier)];
                                vector[1] = sequence[i - Math.Abs((-d - c - b) * multiplier)];
                                vector[2] = sequence[i - Math.Abs((-d - c) * multiplier)];
                                vector[3] = sequence[i - Math.Abs((-d * multiplier))];

                                float[][] currPattern = clusters[((a - 1) * 1000) +
                                                                      ((b - 1) * 100) +
                                                                      ((c - 1) * 10) +
                                                                      ((d - 1))];

                                for (int k = 0; k < currPattern.Length; k++)
                                {
                                    if (distance(vector, currPattern[k]) < error)
                                    {
                                        clustersCount[i]++;
                                        if (distance(vector, currPattern[k]) < currDistance)
                                        {
                                            currDistance = distance(vector, currPattern[k]);
                                            currResult = currPattern[k][4];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                result[i] = currResult;
            }

            return result;
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
            double rmse = Math.Sqrt(mse) * 100;
            return (rmse, nonpred);
            //MessageBox.Show("RMSE:\n" + Convert.ToString(rmse) + "%\nNon-pred.:\n" + Convert.ToString(nonpred));
        }

        public static float distance(float[] vec1, float[] vec2)
        {
            return (float)Math.Sqrt(Math.Pow(vec1[0] - vec2[0], 2) +
                             Math.Pow(vec1[1] - vec2[1], 2) +
                             Math.Pow(vec1[2] - vec2[2], 2) +
                             Math.Pow(vec1[3] - vec2[3], 2));
        }
    }
}
