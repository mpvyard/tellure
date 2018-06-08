using Microsoft.Extensions.Logging;
using ILGPU;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ILGPU.Runtime;
using ILGPU.Runtime.CPU;
using System.Linq;

namespace TSProcessor.CLI.Tasks.Paint
{
    struct Template
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
                //var data = Process(new Template { Distance1 = 1, Distance2 = 1, Distance3 = 1, Distance4 = 1 }, series, clusters);
            }
            var result = PaintCPU(templates, clusters, series, args.Error).ToArray();
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

        //private static int[] Process(Template tempate, float[] series, float[][] clusters)
        //{
        //    using (var context = new Context())
        //    {
        //        using (var accelerator = new CPUAccelerator(context))
        //        {
        //            var paintKernel = accelerator.LoadAutoGroupedStreamKernel<Index, Template, ArrayView<float>, ArrayView2D<float>, ArrayView<int>>(PaintKernel);
        //            using (var seriesBuffer = accelerator.Allocate<float>(series.Count()))
        //            using (var buffer = accelerator.Allocate<int>(series.Count()))
        //            {
        //                seriesBuffer.CopyFrom(series, 0, 0, series.Count());
        //                paintKernel(series.Count(), tempate, seriesBuffer, buffer);

        //                accelerator.Synchronize();

        //                var data = buffer.GetAsArray();
        //                return data;
        //            }
        //        }
        //    }
        //}

        public static ReadOnlySpan<int> PaintCPU(IEnumerable<Template> templates,
            List<float[][]> clusters,
            float[] series,
            double error)
        {
            Span<int> seriesHeatMap = new int[series.Length];
            //TODO: make it run parallel for each template
            int i = 0;
            foreach (var template in templates)
            {
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

        private static void PaintKernel(
            Index index,
            Template template,
            ArrayView<float> series,
            ArrayView2D<float> clusters,
            ArrayView<int> outPutHeatMap)
        {

        }
    }
}
