using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tellure.Algorithms.Forecasting
{
    public delegate void ForecastHandler();
    public class SimpleForecaster
    {
        public static event ForecastHandler OnPointForecasted;

        public static IList<float> ForecastSeries(IList<Template> templates,
            IList<float[][]> clusters,
            ReadOnlySpan<float> data,
            float error,
            int steps)
        {
            var results = Enumerable.Repeat(float.NaN, data.Length).ToArray();

            int maxLength = templates.Max(template => template.Distance1 + template.Distance2 + template.Distance3 + template.Distance4) * 2;
            for (int i = maxLength; i < data.Length - steps; i++)
            {
                float res = ForecastPoint(templates, clusters, data.Slice(i - maxLength, maxLength), error, steps);
                OnPointForecasted();
                results[i + steps] = res;
            }

            return results;
        }

        public static float ForecastPoint(IList<Template> templates,
            IList<float[][]> clusters,
            ReadOnlySpan<float> data,
            float error,
            int steps = 1)
        {
            Span<float> tmp = new float[data.Length + steps - 1];
            data.CopyTo(tmp);

            for (int i = 1; i < steps; i++)
            {
                // -1 Because array indexing from 0
                tmp[data.Length + i - 1] = ForecastNext(templates, clusters, tmp.Slice(0, data.Length + i - 1), error);
            }

            var forecast = ForecastNext(templates, clusters, tmp, error);
            return forecast;
        }

        // Forecast one point further
        private static float ForecastNext(
            IList<Template> templates,
            IList<float[][]> clusters,
            ReadOnlySpan<float> tmp,
            float error)
        {
            float forecastedPoint = float.NaN;
            float forecastintDistance = float.MaxValue;
            int clustersCount = 0;

            var cnt = tmp.Length;
            var realVectorData = new float[4];

            foreach (var template in templates)
            {
                int idx4 = cnt - template.Distance4,
                    idx3 = idx4 - template.Distance3,
                    idx2 = idx3 - template.Distance2,
                    idx1 = idx2 - template.Distance1;

                realVectorData[0] = tmp[idx1];
                realVectorData[1] = tmp[idx2];
                realVectorData[2] = tmp[idx3];
                realVectorData[3] = tmp[idx4];

                var clustersOfTemplate = clusters[templateToIndex(template)];

                foreach (var cluster in clustersOfTemplate)
                {
                    if (DistanceCalculator.Distance(realVectorData, cluster) < error)
                    {
                        clustersCount++;

                        if (DistanceCalculator.Distance(realVectorData, cluster) < forecastintDistance)
                        {
                            forecastintDistance = DistanceCalculator.Distance(realVectorData, cluster);
                            forecastedPoint = cluster[4];
                        }
                    }
                }
            }

            return forecastedPoint;
            //return (currResult, clustersCount);

            // additional local functions
            int templateToIndex(Template tpl)
            {
                // TODO: replace -1 with the lowest template distance-values
                return (tpl.Distance1 - 1) * 1000 +
                        (tpl.Distance2 - 1) * 100 +
                        (tpl.Distance3 - 1) * 10 +
                        (tpl.Distance4 - 1);
            }
        }
    }
}
