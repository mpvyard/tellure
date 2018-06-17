using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Tellure.Entities;

namespace Tellure.Algorithms.Painting
{
    public class CPUPainter
    {
        public static ReadOnlySpan<int> Paint(IList<Template> templates,
                    IList<float[][]> clusters,
                    float[] series,
                    float error)
        {
            Span<int> seriesHeatMap = new int[series.Length];
            for (int i = 0; i < templates.Count; i++)
            {
                var template = templates[i];
                foreach (var cluster in clusters[i])
                {
                    //TODO: refactor this peace of vector creation
                    for (int i5 = template.Distance1 + template.Distance2 + template.Distance3 + template.Distance4,
                       i4 = i5 - template.Distance4,
                       i3 = i5 - template.Distance4 - template.Distance3,
                       i2 = i5 - template.Distance4 - template.Distance3 - template.Distance2,
                       i1 = i5 - template.Distance4 - template.Distance3 - template.Distance2 - template.Distance1;
                       i5 < series.Length - 1; i5++, i4++, i3++, i2++, i1++)
                    {
                        if (Math.Abs(series[i5] - cluster[4]) < 0.1)
                        {
                            var vector = new float[] { series[i1], series[i2], series[i3], series[i4], series[i5] };
                            double distance = DistanceCalculator.Distance(vector, cluster);
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
            return seriesHeatMap;
        }
    }
}
