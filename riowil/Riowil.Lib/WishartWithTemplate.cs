using System;
using System.Collections.Generic;
using System.Text;

namespace Riowil.Lib
{
    public class Wishart
    {
        public static IEnumerable<double[]> GenerateSequenceForWishart(double[] sequence)
        {
            int sum = 0;
            int[] distance = new int[4];
            for (int a = 1; a <= 10; a++)
            {
                for (int b = 1; b <= 10; b++)
                {
                    for (int c = 1; c <= 10; c++)
                    {
                        for (int d = 1; d <= 10; d++)
                        {
                            distance[0] = a;
                            distance[1] = a + b;
                            distance[2] = a + b + c;
                            distance[3] = a + b + c + d;
                            sum = a + b + c + d;

                            List<double> SequenceForWishart = new List<double>();
                            for (int i = 0; i < sequence.Length - sum; i++)
                            {
                                SequenceForWishart.Add(sequence[i]);
                                SequenceForWishart.Add(sequence[i + distance[0]]);
                                SequenceForWishart.Add(sequence[i + distance[1]]);
                                SequenceForWishart.Add(sequence[i + distance[2]]);
                                SequenceForWishart.Add(sequence[i + distance[3]]);
                            }
                            yield return SequenceForWishart.ToArray();
                        }
                    }
                }
            }
        }

        public static IEnumerable<int[]> GenerateTemplateForWishart()
        {
            for (int a = 1; a <= 10; a++)
            {
                for (int b = 1; b <= 10; b++)
                {
                    for (int c = 1; c <= 10; c++)
                    {
                        for (int d = 1; d <= 10; d++)
                        {
                            yield return new int[] { a, b, c, d };
                        }
                    }
                }
            }
        }
    }
}
