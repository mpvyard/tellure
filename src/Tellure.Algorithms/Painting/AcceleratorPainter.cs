using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using ILGPU.Runtime.Cuda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tellure.Algorithms.Painting
{
    public class AcceleratorPainter
    {
        public static int[] Paint(IList<Template> templates,
            IList<float[][]> clusters,
            float[] series,
            float error)
        {
            var result = new int[series.Length];
            for (int i = 0; i < templates.Count; i++)
            {
                var data = Process(templates[i], series, clusters[i], error);
                for (int j = 0; j < data.Length; j++)
                {
                    result[j] += data[j];
                }
            }

            return result;
        }

        private static int[] Process(Template tempate, float[] series, float[][] clusters, float error)
        {
            var clustersHeight = clusters.Length;
            var clustersWidth = clusters[0].Length;
            var clusters2d = new float[clustersHeight, clustersWidth];
            for (int i = 0; i < clustersHeight; i++)
            {
                for (int j = 0; j < clustersWidth; j++)
                {
                    clusters2d[i, j] = clusters[i][j];
                }
            }

            using (var context = new Context())
            {
                using (var accelerator = new CPUAccelerator(context))
                {
                    var paintKernel = accelerator.LoadAutoGroupedStreamKernel<Index, Template, ArrayView<float>, ArrayView2D<float>, ArrayView<int>, float>(PaintKernel);
                    using (var seriesBuffer = accelerator.Allocate<float>(series.Count()))
                    using (var clustersBuffer = accelerator.Allocate<float>(clustersHeight, clustersWidth))
                    using (var buffer = accelerator.Allocate<int>(series.Count()))
                    {
                        seriesBuffer.CopyFrom(series, 0, 0, series.Count());
                        clustersBuffer.CopyFrom(clusters2d, new Index2(0, 0), new Index2(0, 0), new Index2(clustersHeight, clustersWidth));
                        paintKernel(clustersHeight, tempate, seriesBuffer, clustersBuffer, buffer, error);

                        accelerator.Synchronize();

                        var data = buffer.GetAsArray();
                        return data;
                    }
                }
            }
        }

        private static void PaintKernel(
            Index i,
            Template template,
            ArrayView<float> series,
            ArrayView2D<float> clusters,
            ArrayView<int> outPutHeatMap,
            float error)
        {
            //TODO: refactor this peace of vector creation
            for (int i5 = template.Distance1 + template.Distance2 + template.Distance3 + template.Distance4,
                i4 = i5 - template.Distance4,
                i3 = i5 - template.Distance4 - template.Distance3,
                i2 = i5 - template.Distance4 - template.Distance3 - template.Distance2,
                i1 = i5 - template.Distance4 - template.Distance3 - template.Distance2 - template.Distance1;
                i5 < series.Length - 1; i5++, i4++, i3++, i2++, i1++)
            {
                if (Math.Abs(series[i5] - clusters[i, 4]) < 0.1)
                {
                    var vector = new float[] { series[i1], series[i2], series[i3], series[i4], series[i5] };
                    var cluster = new float[] { clusters[i, 0], clusters[i, 1], clusters[i, 2], clusters[i, 3], clusters[i, 4] };
                    double distance = DistanceCalculator.Distance(vector, cluster);
                    if (distance <= error)
                    {
                        outPutHeatMap[i1]++;
                        outPutHeatMap[i2]++;
                        outPutHeatMap[i3]++;
                        outPutHeatMap[i4]++;
                    }
                }
            }
        }
    }
}
