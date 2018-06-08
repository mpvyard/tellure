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
    public delegate void SequenceHandler<T>(IEnumerable<T> sequence);
    static partial class Clusterizer
    {
        private const float sigma = 10f;
        private const float r = 26f;
        private const float b = 2.666f;

        private const float generationStep = 0.05f;
        private const int skipCount = 3000;
        private const int sequenceCount = 13500;//16500

        private static readonly Vector3 Y02 = new Vector3(10, -1, 1);


        public static event SequenceHandler<float> AfterClusterize1dSequenceGenerated;
        public static event SequenceHandler<Vector3> AfterClusterize3dSequenceGenerated;


        private static FileWriter fileWriter;
        public static int Clusterize(ClusterizationOptions opts, ILogger logger, FileWriter writer)
        {
            fileWriter = writer;
            logger.LogInformation("Operation started...");

            logger.LogInformation("Generate time-series");
            TimeSeriesGenerator lr = new TimeSeriesGenerator(sigma, r, b);
            IEnumerable<Vector3> sequence = lr.Generate(Y02, generationStep, skipCount + sequenceCount)
                .Skip(skipCount);

            if (opts.Dimentions == 1)
            {
                logger.LogInformation("Normalize generated series");
                var series = sequence.Select(vector => vector.X).ToList();
                series = SeriesNormalizer.Normalize(series).ToList();

                logger.LogInformation("Start clusterization");
                ClusterizeAllParallel(series, opts.From.ToArray(), opts.To.ToArray(), logger);
                logger.LogInformation("Operation completed");
                return 0;
            }

            if (opts.Dimentions == 3)
            {
                logger.LogInformation("Normalize generated series");
                List<Vector3> NormalizedSequence = SeriesNormalizer.Normalize(sequence).ToList();

                logger.LogInformation("Start clusterization");
                ClusterizeAllParallel(NormalizedSequence, opts.From.ToArray(), opts.To.ToArray(), logger);
                logger.LogInformation("Operation completed");
                return 0;
            }

            throw new ArgumentOutOfRangeException(nameof(opts.Dimentions));
        }

        private static void ClusterizeAllParallel(IReadOnlyList<float> series, int[] from, int[] to, ILogger logger)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = ZVectorBuilder.Build(series, template, 0);

                logger.LogInformation("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);

                logger.LogInformation("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.json");
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
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
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
        }
    }
}
