using Microsoft.Extensions.Logging;
using Riowil.Entities;
using Riowil.Entities.Clusters;
using Riowil.Lib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Tellure.Lib;

namespace TSProcessor.CLI.Tasks.Clusterize
{
    class ProgramClusterize3d
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
            List<Vector3> dots = new List<Vector3>();
            dots = NormalizedSequence.ToList();
            //foreach (Vector3 var in NormalizedSequence)
            //{
            //    dots.Add(var);
            //}
            Series3d series = new Series3d(seriesParams, dots);

            logger.LogInformation("Start clusterization");
            ClusterizeAllParallel(series, opts.From, opts.To, logger);
            logger.LogInformation("Operation completed");
            return 0;
        }

        private static void ClusterizeAllParallel(Series3d series, int[] from, int[] to, ILogger logger)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = FindZVectors(series, template, 0).ToList();

                logger.LogInformation("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);

                logger.LogInformation("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.json");
                using (var writer = new StreamWriter(path))
                {
                    ServiceStack.Text.JsonSerializer.SerializeToWriter(clusters, writer);
                }

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.centers.json");
                using (var writer = new StreamWriter(pathCenters))
                {
                    ServiceStack.Text.JsonSerializer.SerializeToWriter(centers, writer);
                }
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
        }

        private static IEnumerable<ZVector3d> FindZVectors(Series3d series, int[] step, int firstNumber)
        {
            List<Vector3> teachSeries = series.GetTeachSeries();
            IEnumerable<ZVector3d> zVectors = ZVectorBuilder3d.Build(teachSeries, step, firstNumber);
            return zVectors;
        }

        private static IEnumerable<InitialCluster3d> Clusterize(List<ZVector3d> zVectors)
        {
            WishartAlgor3d algor = new WishartAlgor3d(new WishartParams { H = 0.2, K = 11 });
            //List<InitialCluster> clusters = algor.Clusterize(zVectors);
            List<InitialCluster3d> clusters = algor.Clusterize(zVectors);
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
    }
}
