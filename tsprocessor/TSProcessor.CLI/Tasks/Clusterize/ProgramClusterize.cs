using Microsoft.Extensions.Logging;
using Riowil.Entities;
using Riowil.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Tellure.Lib;

namespace TSProcessor.CLI.Tasks.Clusterize
{
    static class Clusterizer
    {
        public static int Clusterize(ClusterizationOptions opts, ILogger logger)
        {
            logger.LogInformation("Operation started...");

            logger.LogInformation("Generate time-series");
            SeriesParams seriesParams = GetCurrentParams();
            Vector3 Y02 = new Vector3(10, -1, 1);
            TimeSeriesGenerator lr = new TimeSeriesGenerator(10, 28, 2.666f);
            IEnumerable<Vector3> sequence = lr.Generate(Y02, 0.05f, 3000, 100000);

            logger.LogInformation("Normalize generated series");
            IEnumerable<Vector3> NormalizedSequence = SeriesNormalizer.Normalize(sequence);
            //Some convertion here to series
            List<double> dots = new List<double>();
            foreach (Vector3 var in NormalizedSequence)
            {
                dots.Add(var.X);
                //dots.Add(var.Y);
                //dots.Add(var.Z);
            }
            Series series = new Series(seriesParams, dots);

            logger.LogInformation("Start clusterization");
            ClusterizeAllParallel(series, opts.From, opts.To, logger);
            logger.LogInformation("Operation completed");
            return 0;
        }

        private static void ClusterizeAllParallel(Series series, int[] from, int[] to, ILogger logger)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = FindZVectors(series, template, series.Points.Count).ToList();

                logger.LogInformation("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);

                logger.LogInformation("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.json");
                using (var writer = new StreamWriter(path))
                {
                    ServiceStack.Text.JsonSerializer.SerializeToWriter(clusters, writer);
                }

                var centers = clusters.Select(cluster => cluster.Centr.List).ToList();
                var pathCenters = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.centers.json");
                using (var writer = new StreamWriter(pathCenters))
                {
                    ServiceStack.Text.JsonSerializer.SerializeToWriter(centers, writer);
                }
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
        }

        private static void ClusterizeAll(Series series, int[] from, int[] to, ILogger logger)
        {
            foreach (var template in Wishart.GenerateTemplateForWishart(from, to))
            {
                var vectors = FindZVectors(series, template, series.Points.Count).ToList();
                var clusters = Clusterize(vectors);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{template[0]}-{template[1]}-{template[2]}-{template[3]}.json");
                using (var writer = new StreamWriter(path))
                {
                    ServiceStack.Text.JsonSerializer.SerializeToWriter(clusters, writer);
                }
            }
        }

        private static IEnumerable<InitialCluster> Clusterize(List<ZVector> zVectors)
        {
            IClusterizeAlgor algor = new WishartAlgor(new WishartParams { H = 0.2, K = 11 });
            List<InitialCluster> clusters = algor.Clusterize(zVectors);
            return clusters;
        }

        private static SeriesParams GetCurrentParams()
        {
            SeriesType seriesType = SeriesType.Lorence;
            return new SeriesParams
            {
                Category = "",
                FirstInTraining = 0,
                CountInTraining = 10000,
                FirstInTest = 7000,
                CountInTest = 3000,
                FistInCheck = 23000,
                CountInCheck = 500,
                Type = seriesType
            };
        }

        private static IEnumerable<ZVector> FindZVectors(Series series, int[] step, int firstNumber)
        {
            List<double> teachSeries = series.GetTeachSeries();
            IEnumerable<ZVector> zVectors = ZVectorBuilder.Build(teachSeries, step, firstNumber);
            return zVectors;
        }
    }
}
