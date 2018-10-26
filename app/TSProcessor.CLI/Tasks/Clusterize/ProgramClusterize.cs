using Microsoft.Extensions.Logging;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Tellure.Algorithms;
using Tellure.Generator;

namespace TSProcessor.CLI.Tasks.Clusterize
{
    public delegate void SequenceHandler();
    static partial class Clusterizer
    {
        public static event SequenceHandler OnClusterization1dCompleted;
        public static event SequenceHandler OnClusterization3dCompleted;

        private static FileWriter fileWriter;
        public static int Clusterize(ClusterizationOptions opts, ILogger logger, FileWriter writer)
        {
            opts.SeriesFileName = opts.SeriesFileName ?? DefaultParams.seriesPath;
            opts.ClustersDirectory = opts.ClustersDirectory ?? DefaultParams.clustersPath;

            string centersDir = Path.Join(opts.ClustersDirectory, "centers");
            string fullDir = Path.Join(opts.ClustersDirectory, "full");

            if(!File.Exists(opts.SeriesFileName))
            {
                logger.LogError("File with series {series} doesn't exist", opts.SeriesFileName);
                return 1;
            }

            if(!Directory.Exists(opts.ClustersDirectory))
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

            if (opts.Dimentions == 1)
            {
                List<float> series;
                using (var stream = new StreamReader(opts.SeriesFileName))
                {
                    series = ServiceStack.Text.JsonSerializer.DeserializeFromReader<List<float>>(stream);
                }
                logger.LogInformation("Start clusterization");
                var options = new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Yellow,
                    ForegroundColorDone = ConsoleColor.DarkGreen,
                    BackgroundColor = ConsoleColor.DarkGray,
                    BackgroundCharacter = '\u2593',
                    DisplayTimeInRealTime = false
                };

                // TODO: calc real number of clusters, intead of this stub
                var totalTicks = 10000;

                using (var pbar = new ProgressBar(totalTicks, "Progress of clusterization", options))
                {
                    void onClusterization1dCompleted()
                    {
                        pbar.Tick();
                    }

                    OnClusterization1dCompleted += onClusterization1dCompleted;

                    ClusterizeAllParallel(series, opts.From.ToArray(), opts.To.ToArray(), logger, opts.ClustersDirectory);

                    OnClusterization1dCompleted -= onClusterization1dCompleted;
                }
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

                var options = new ProgressBarOptions
                {
                    ForegroundColor = ConsoleColor.Yellow,
                    ForegroundColorDone = ConsoleColor.DarkGreen,
                    BackgroundColor = ConsoleColor.DarkGray,
                    BackgroundCharacter = '\u2593',
                    DisplayTimeInRealTime = false
                };

                // TODO: calc real number of clusters, intead of this stub
                var totalTicks = 10000;

                using (var pbar = new ProgressBar(totalTicks, "Progress of clusterization", options))
                {
                    void onClusterization3dCompleted()
                    {
                        pbar.Tick();
                    }

                    OnClusterization3dCompleted += onClusterization3dCompleted;

                    ClusterizeAllParallel(series, opts.From.ToArray(), opts.To.ToArray(), logger, opts.ClustersDirectory);

                    OnClusterization3dCompleted -= onClusterization3dCompleted;
                }
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

                logger.LogDebug("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);
                OnClusterization1dCompleted();

                logger.LogDebug("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(clustersDir, "full", $"{stringTemplate}.json");
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(clustersDir, "centers", $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogDebug("Writing finished for {template}", stringTemplate);
            });
        }


        private static void ClusterizeAllParallel(IReadOnlyList<Vector3> series, int[] from, int[] to, ILogger logger, string clustersDir)
        {
            Wishart.GenerateTemplateForWishart(from, to).AsParallel().ForAll(template =>
            {
                string stringTemplate = $"{template[0]}-{template[1]}-{template[2]}-{template[3]}";
                var vectors = ZVectorBuilder3d.Build(series, template, 0);

                logger.LogDebug("Clusterize for template {template}", stringTemplate);
                var clusters = Clusterize(vectors);
                OnClusterization3dCompleted();

                logger.LogDebug("Start writing results for {template} to file", stringTemplate);
                var path = Path.Combine(clustersDir, "full", $"{stringTemplate}.json");
                fileWriter.Write(clusters, path);

                var centers = clusters.Select(cluster => cluster.Centr).ToList();
                var pathCenters = Path.Combine(clustersDir, "centers", $"{stringTemplate}.centers.json");
                fileWriter.Write(centers, pathCenters);
                logger.LogDebug("Writing finished for {template}", stringTemplate);
            });
        }
    }
}
