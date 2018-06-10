using Microsoft.Extensions.Logging;
using ILGPU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System.Linq;
using ILGPU.Runtime.Cuda;

namespace TSProcessor.CLI.Tasks.Paint
{
    public struct Template
    {
        public int Distance1;
        public int Distance2;
        public int Distance3;
        public int Distance4;
    }

    static class Painter
    {
        public static int Paint(PaintOptions args, FileWriter writer, ILogger logger)
        {
            args.SeriesFileName = args.SeriesFileName ?? DefaultParams.seriesPath;
            args.ClustersDirectory = args.ClustersDirectory ?? DefaultParams.clustersPath;
            if (!File.Exists(args.SeriesFileName))
            {
                logger.LogError("File with series {series} doesn't exist", args.SeriesFileName);
                return 1;
            }

            if (!Directory.Exists(args.ClustersDirectory))
            {
                logger.LogError("Directory with clusters {clusters} doesn't exist", args.ClustersDirectory);
                return 1;
            }

            float[] series;
            using (var stream = new StreamReader(args.SeriesFileName))
            {
                series = ServiceStack.Text.JsonSerializer.DeserializeFromReader<float[]>(stream);
            }
            List<float[][]> clusters = new List<float[][]>();
            List<Template> templates = new List<Template>();
            foreach (var file in Directory.GetFiles(args.ClustersDirectory))
            {
                using (var stream = new StreamReader(file))
                {
                    var template = GetTemplateFromString(file);
                    var templateClusters = ServiceStack.Text.JsonSerializer.DeserializeFromReader<float[][]>(stream);
                    templates.Add(template);
                    clusters.Add(templateClusters);
                }
            }
            //var result = Process(templates, series, clusters, args.Error);

            var result = new int[series.Length];
            for (int i = 0; i < templates.Count; i++)
            {
                var data = Process(templates[i], series, clusters[i], args.Error);
                for (int j = 0; j < data.Length; j++)
                {
                    result[j] += data[j];
                }
            }

            //var result = PaintCPU(templates, clusters, series, args.Error).ToArray();
            writer.Write(result, DefaultParams.paintPath);
            return 0;
        }


        private static Template GetTemplateFromString(string template)
        {
            string name = Path.GetFileName(template);
            int ln = name.IndexOf('.');
            string temp = name.Substring(0, ln);
            string[] parts = temp.Split('-');
            int[] numbers = parts.Where(x =>
                int.TryParse(x, out var _)
                ).Select(x => int.Parse(x)).ToArray();

            return new Template
            {
                Distance1 = numbers[0],
                Distance2 = numbers[1],
                Distance3 = numbers[2],
                Distance4 = numbers[3]
            };
        }

        public static ReadOnlySpan<int> PaintCPU(IList<Template> templates,
            List<float[][]> clusters,
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
            return seriesHeatMap;
        }

        private static double Distance(float[] vector, float[] cluster)
        {
            var count = vector.Length;
            double dist = 0;
            for (int i = 0; i < count; i++)
            {
                dist += Math.Pow(cluster[i] - vector[i], 2);
            }
            return Math.Sqrt(dist);
        }
        #region ProcessGPUPartial
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
                    double distance = Distance(vector, cluster);
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
        #endregion
        #region ProcessGPUFull
        private static int[] Process(IList<Template> tempate, float[] series, IList<float[][]> clusterCollection, float error)
        {
            var templatesCount = clusterCollection.Count;
            var clustersCount = clusterCollection.Max(x => x.Length);
            var pointsInClusters = clusterCollection[0][0].Length;
            float[,,] clsts = new float[templatesCount, clustersCount, pointsInClusters];
            for(int k = 0; k < templatesCount; k++)
            {
                for (int i = 0; i < clustersCount; i++)
                {
                    if(i >= clusterCollection[k].Length)
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
                if(cluster[j] != 0f)
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
                        double distance = Distance(vector, cluster);
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
        #endregion
    }
}
