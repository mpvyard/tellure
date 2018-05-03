﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tellure.Lib
{
    public static class SeriesNormalizer
    {
        public static IEnumerable<double> Normalize(this IEnumerable<double> data)
        {
            double dataMax = data.Max();
            double dataMin = data.Min();
            double range = dataMax - dataMin;

            return data.Select(x => 2 * (x - ((dataMin + dataMax) / 2)) / (dataMax - dataMin));
        }

        public static Span<double> Normalize(this Span<double> data)
        {
            var (dataMin, dataMax) = getMinAndMax(data);
            double range = dataMax - dataMin;

            Span<double> normalizedData = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                normalizedData[i] = 2 * (data[i] - ((dataMin + dataMax) / 2))
                    / (dataMax - dataMin);
            }

            return normalizedData;

            (double min, double max) getMinAndMax(ReadOnlySpan<double> collection)
            {
                var max = collection[0];
                var min = collection[0];
                foreach (var number in collection)
                {
                    if (number > max)
                    {
                        max = number;
                    }

                    if (number < min)
                    {
                        min = number;
                    }
                }

                return (min, max);
            }
        }

       
    }
}
