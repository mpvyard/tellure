﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

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

        public static IEnumerable<float> Normalize(this IEnumerable<float> data)
        {
            float dataMax = data.Max();
            float dataMin = data.Min();
            float range = dataMax - dataMin;

            return data.Select(x => 2 * (x - ((dataMin + dataMax) / 2)) / (dataMax - dataMin));
        }

        public static List<Vector3> Normalize (this List<Vector3> data)
        {
            Vector3 dataMax;
            dataMax = data.First();
            Vector3 Average = new Vector3(0, 0, 0);

            foreach (Vector3 vect in data)
            {
                if (vect.Length() > dataMax.Length())
                {
                    dataMax = vect;
                }
                Average += vect;
            }
            Average = Average / data.Count();

            for (int i = 0; i < data.Count(); ++i)
            {
                data[i] = (data[i] - Average) / dataMax.Length();
            }

            return data;
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

        public static Span<float> Normalize(this Span<float> data)
        {
            var (dataMin, dataMax) = getMinAndMax(data);
            double range = dataMax - dataMin;

            Span<float> normalizedData = new float[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                normalizedData[i] = 2 * (data[i] - ((dataMin + dataMax) / 2))
                    / (dataMax - dataMin);
            }

            return normalizedData;

            (float min, float max) getMinAndMax(ReadOnlySpan<float> collection)
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