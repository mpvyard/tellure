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
        public static event SequenceHandler<float> AfterClusterize1dSequenceGenerated;
        public static event SequenceHandler<Vector3> AfterClusterize3dSequenceGenerated;

        private static FileWriter fileWriter;
        public static int Clusterize(ClusterizationOptions opts, ILogger logger, FileWriter writer)
        {
            opts.SeriesFileName = opts.SeriesFileName ?? DefaultParams.seriesPath;
            opts.ClustersDirectory = opts.ClustersDirectory ?? DefaultParams.clustersPath;

            if(!File.Exists(opts.SeriesFileName))
            {
                logger.LogError("File with series {series} doesn't exist", opts.SeriesFileName);
                return 1;
            }

            if(!Directory.Exists(opts.ClustersDirectory))
            {
                logger.LogWarning("Directory {dir} doesn't exist, creating", opts.ClustersDirectory);
                Directory.CreateDirectory(opts.ClustersDirectory);
            }

            fileWriter = writer;

            if (opts.Dimentions == 1)
            {
                List<float> series;
                using (var stream = new StreamReader(opts.SeriesFileName))
                {
                    series = ServiceStack.Text.JsonSerializer.DeserializeFromReader<List<float>>(stream);
                }
                logger.LogInformation("Start clusterization");
                ClusterizeAllParallel(series, opts.From.ToArray(), opts.To.ToArray(), logger, opts.ClustersDirectory);
                logger.LogInformation("Operation completed");
                return 0;
            }

            if (opts.Dimentions == 3)
            {
                logger.LogInformation("Operation started...");

                logger.LogInformation("Generate time-series");
                TimeSeriesGenerator lr = new TimeSeriesGenerator(DefaultParams.sigma, DefaultParams.r, DefaultParams.b);
                IEnumerable<Vector3> sequence = lr.Generate(DefaultParams.Y0, DefaultParams.generationStep, DefaultParams.skipCount + DefaultParams.sequenceCount)
                    .Skip(DefaultParams.skipCount);
                logger.LogInformation("Normalize generated series");
                List<Vector3> series = SeriesNormalizer.Normalize(sequence).ToList();
                //List<Vector3> series; - deserialization of vector3 is broken out
                //using (var stream = new StreamReader(opts.SeriesFileName))
                //{
                //    series = ServiceStack.Text.JsonSerializer.DeserializeFromReader<List<Vector3>>(stream);
                //}
                logger.LogInformation("Start clusterization");
                ClusterizeAllParallel(series, opts.From.ToArray(), opts.To.ToArray(), logger, opts.ClustersDirectory);
                logger.LogInformation("Operation completed");
                return 0;
            }

            throw new ArgumentOutOfRangeException(nameof(opts.Dimentions));
        }

        private static void ClusterizeAllParallel(IReadOnlyList<float> series, int[] from, int[] to, ILogger logger, string clustersDir)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = ZVectorBuilder.Build(series, template, 0);

                logger.LogInformation("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);

                logger.LogInformation("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(clustersDir, $"{stringTemplate}.json");
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(clustersDir, $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
        }


        private static void ClusterizeAllParallel(IReadOnlyList<Vector3> series, int[] from, int[] to, ILogger logger, string clustersDir)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = ZVectorBuilder3d.Build(series, template, 0);

                logger.LogInformation("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);

                logger.LogInformation("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(clustersDir, $"{stringTemplate}.json");
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(clustersDir, $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogInformation("Writing finished for {template}", stringTemplate);
            });
        }
    }
}
