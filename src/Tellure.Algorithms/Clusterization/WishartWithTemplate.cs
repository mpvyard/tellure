using System;
using System.Collections.Generic;
using System.Text;

namespace Tellure.Algorithms
{
    public class Wishart
    {
        // TODO: maybe rework it to work with range like 1.1.1.1 - 2.1.1.1
        // and produce 1000 results, but not 1
        public static IEnumerable<int[]> GenerateTemplateForWishart(int[] from, int[] to)
        {
            for (int a = from[0]; a <= to[0]; a++)
            {
                for (int b = from[1]; b <= to[1]; b++)
                {
                    for (int c = from[2]; c <= to[2]; c++)
                    {
                        for (int d = from[3]; d <= to[3]; d++)
                        {
                            yield return new int[] { a, b, c, d };
                        }
                    }
                }
            }
        }
    }
}
