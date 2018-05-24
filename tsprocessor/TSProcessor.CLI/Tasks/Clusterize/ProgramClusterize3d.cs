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
            Vector3 Y02 = new Vector3(10, -1, 1);
            TimeSeriesGenerator lr = new TimeSeriesGenerator(10, 28, 2.666f);
            IEnumerable<Vector3> sequence = lr.Generate(Y02, 0.05f, 3000, 13500);

            logger.LogInformation("Normalize generated series");
            List<Vector3> NormalizedSequence = SeriesNormalizer.Normalize(sequence).ToList();

            logger.LogInformation("Start clusterization");
            ClusterizeAllParallel(NormalizedSequence, opts.From, opts.To, logger);
            logger.LogInformation("Operation completed");
            return 0;
        }

        private static void ClusterizeAllParallel(IReadOnlyList<Vector3> series, int[] from, int[] to, ILogger logger)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = ZVectorBuilder3d.Build(series, template, 0);

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

        private static IEnumerable<InitialCluster3d> Clusterize(IReadOnlyList<ZVector3d> zVectors)
        {
            WishartAlgor3d algor = new WishartAlgor3d(new WishartParams { H = 0.2, K = 11 });
            var clusters = algor.Clusterize(zVectors);
            return clusters;
        }
    }
}
