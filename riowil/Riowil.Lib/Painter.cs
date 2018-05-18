using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Riowil.Lib
{
    class Painter
    {
        public void Paint(IEnumerable<(int Distance1, int Distance2, int Distance3, int Distance4)> templates,
            IEnumerable<Vector<double>> clusters,
            ReadOnlySpan<double> series,
            double error)
        {
            Span<int> seriesHeatMap = new int[series.Length];
            //TODO: make it run parallel for each template
            foreach (var template in templates)
            {
                foreach (var cluster in clusters)
                {
                    //TODO: refactor this peace of vector creation
                    for (int i5 = template.Distance1 + template.Distance2 + template.Distance3 + template.Distance4 - 1,
                       i4 = i5 - template.Distance4,
                       i3 = i5 - template.Distance4 - template.Distance3,
                       i2 = i5 - template.Distance4 - template.Distance3 - template.Distance2,
                       i1 = i5 - template.Distance4 - template.Distance3 - template.Distance2 - template.Distance1;
                       i5 < series.Length - 1; i5++, i4++, i3++, i2++, i1++)
                    {
                        var vector = new Vector<double>(new double[] { series[i1], series[i2], series[i3], series[i4], series[i5] });
                        double distance = Distance(vector, cluster);
                        //TODO: check that i5 is good enough to forecast,
                        //if it is - check that template itself is about series 
                        if (distance <= error)
                        {
                            seriesHeatMap[i1]++;
                            seriesHeatMap[i2]++;
                            seriesHeatMap[i3]++;
                            seriesHeatMap[i4]++;
                        }
                    }
                }
            }
        }

        private double Distance(Vector<double> vector, Vector<double> cluster)
        {
            var count = Vector<double>.Count;
            double dist = 0;
            for (int i = 0; i < count; i++)
            {
                dist += Math.Pow(cluster[i] - vector[i], 2);
            }
            return Math.Sqrt(dist);
        }
    }
}
