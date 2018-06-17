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
    public class AcceleratorPainterExperimental
    {
        public static int[] Process(IList<Template> tempate,
            IList<float[][]> clusterCollection,
            float[] series,
            float error)
        {
            var templatesCount = clusterCollection.Count;
            var clustersCount = clusterCollection.Max(x => x.Length);
            var pointsInClusters = clusterCollection[0][0].Length;
            float[,,] clsts = new float[templatesCount, clustersCount, pointsInClusters];
            for (int k = 0; k < templatesCount; k++)
            {
                for (int i = 0; i < clustersCount; i++)
                {
                    if (i >= clusterCollection[k].Length)
                    {
                        break;
                    }

                    for (int j = 0; j < pointsInClusters; j++)
                    {
                        clsts[k, i, j] = clusterCollection[k][i][j];
                    }
                }
            }

            using (var context = new Context())
            {
                using (var accelerator = new CPUAccelerator(context))
                {
                    var paintKernel = accelerator.LoadAutoGroupedStreamKernel<Index2, ArrayView<Template>, ArrayView<float>, ArrayView3D<float>, ArrayView<int>, float>(PaintKernel2d);
                    using (var seriesBuffer = accelerator.Allocate<float>(series.Count()))
                    using (var templatesBuffer = accelerator.Allocate<Template>(templatesCount))
                    using (var clustersBuffer = accelerator.Allocate<float>(templatesCount, clustersCount, pointsInClusters))
                    using (var buffer = accelerator.Allocate<int>(series.Count()))
                    {
                        seriesBuffer.CopyFrom(series, 0, 0, series.Count());
                        clustersBuffer.CopyFrom(clsts, new Index3(0, 0, 0), new Index3(0, 0, 0), new Index3(templatesCount, clustersCount, pointsInClusters));
                        paintKernel(new Index2(templatesCount, clustersCount), templatesBuffer, seriesBuffer, clustersBuffer, buffer, error);

                        accelerator.Synchronize();

                        var data = buffer.GetAsArray();
                        return data;
                    }
                }
            }
        }
        private static void PaintKernel2d(
           Index2 i,
           ArrayView<Template> templates,
           ArrayView<float> series,
           ArrayView3D<float> clusters,
           ArrayView<int> outPutHeatMap,
           float error)
        {
            var template = templates[i.X];
            var cluster = new float[] { clusters[i.X, i.Y, 0], clusters[i.X, i.Y, 1], clusters[i.X, i.Y, 2], clusters[i.X, i.Y, 3], clusters[i.X, i.Y, 4] };

            bool isExtra = true;
            for (int j = 0; j < cluster.Length; j++)
            {
                if (cluster[j] != 0f)
                {
                    isExtra = false;
                }
            }

            if (!isExtra)
            {
                for (int i5 = template.Distance1 + template.Distance2 + template.Distance3 + template.Distance4,
                    i4 = i5 - template.Distance4,
                    i3 = i5 - template.Distance4 - template.Distance3,
                    i2 = i5 - template.Distance4 - template.Distance3 - template.Distance2,
                    i1 = i5 - template.Distance4 - template.Distance3 - template.Distance2 - template.Distance1;
                    i5 < series.Length - 1; i5++, i4++, i3++, i2++, i1++)
                {
                    if (Math.Abs(series[i5] - clusters[i.X, i.Y, 4]) < 0.1)
                    {
                        var vector = new float[] { series[i1], series[i2], series[i3], series[i4], series[i5] };
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
}
