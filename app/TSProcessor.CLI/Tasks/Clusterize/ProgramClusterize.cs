using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Tellure.Algorithms;
using Tellure.Generator;
using TSProcessor.CLI.IO;

namespace TSProcessor.CLI.Tasks.Clusterize
{
    public delegate void SequenceHandler<T>(IEnumerable<T> sequence);
    static partial class Clusterizer
    {
        private static FileWriter fileWriter;
        public static int Clusterize(ClusterizationOptions opts, ILogger logger, FileReader reader, FileWriter writer)
        {
            opts.SeriesFileName = opts.SeriesFileName ?? DefaultParams.seriesPath;
            opts.ClustersDirectory = opts.ClustersDirectory ?? DefaultParams.clustersPath;
            string centersDir = Path.Join(opts.ClustersDirectory, "centers");
            string fullDir = Path.Join(opts.ClustersDirectory, "full");

            if (!File.Exists(opts.SeriesFileName))
            {
                logger.LogError("File with series {series} doesn't exist", opts.SeriesFileName);
                return 1;
            }

            if(!Directory.Exists(opts.ClustersDirectory))
            {
                logger.LogWarning("Directory {dir} doesn't exist, creating", opts.ClustersDirectory);
                Directory.CreateDirectory(opts.ClustersDirectory);
            }

            if (!Directory.Exists(opts.ClustersDirectory))
            {
                logger.LogWarning("Directory {dir} doesn't exist, creating", opts.ClustersDirectory);
                Directory.CreateDirectory(opts.ClustersDirectory);
                Directory.CreateDirectory(fullDir);
                Directory.CreateDirectory(centersDir);
            }

            if (!Directory.Exists(centersDir))
            {
                logger.LogWarning("Directory {dir} doesn't exist, creating", centersDir);
                Directory.CreateDirectory(centersDir);
            }

            if (!Directory.Exists(fullDir))
            {
                logger.LogWarning("Directory {dir} doesn't exist, creating", fullDir);
                Directory.CreateDirectory(fullDir);
            }

            fileWriter = writer;

            var series = reader.Read<Vector4[]>(opts.SeriesFileName);

            logger.LogInformation("Start clusterization");
            ClusterizeAllParallel(series, opts.From.ToArray(), opts.To.ToArray(), logger, opts.ClustersDirectory);
            logger.LogInformation("Operation completed");
            return 0;
        }

        private static void ClusterizeAllParallel(IReadOnlyList<Vector4> series, int[] from, int[] to, ILogger logger, string clustersDir)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = ZVectorBuilder.Build(series, template, 0);

                logger.LogInformation("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);

                logger.LogInformation("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(clustersDir, "full", $"{stringTemplate}.json");
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(clustersDir, "centers", $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
        }
    }
}
