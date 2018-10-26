using Microsoft.Extensions.Logging;
using ShellProgressBar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tellure.Algorithms;
using Tellure.Algorithms.Forecasting;

namespace TSProcessor.CLI.Tasks.Forecast
{
    class Forecaster
    {
        public static int Forecast(ForecastOptions args, FileWriter writer, ILogger logger)
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
            logger.LogInformation("Reading clusters...");
            foreach (var file in Directory.GetFiles(args.ClustersDirectory))
            {
                using (var stream = new StreamReader(file))
                {
                    var template = Template.Parse(file);
                    var templateClusters = ServiceStack.Text.JsonSerializer.DeserializeFromReader<float[][]>(stream);
                    templates.Add(template);
                    clusters.Add(templateClusters);
                }
            }
            logger.LogInformation("Clusters are read succesfully");
            var options = new ProgressBarOptions
            {
                ForegroundColor = ConsoleColor.Yellow,
                ForegroundColorDone = ConsoleColor.DarkGreen,
                BackgroundColor = ConsoleColor.DarkGray,
                BackgroundCharacter = '\u2593',
                DisplayTimeInRealTime = false
            };

            // Temporary stub
            Span<float> tmp = series.Skip(series.Length - 1000).ToArray();
            var totalTicks = tmp.Length - 80 - args.ForecastringSteps;

            IList<float> results;
            using (var pbar = new ProgressBar(totalTicks, "Progress of forecasting", options))
            {
                SimpleForecaster.OnPointForecasted += () =>
                {
                    pbar.Tick();
                };

                results = SimpleForecaster.ForecastSeries(templates, clusters, tmp, args.Error, args.ForecastringSteps);
                // TODO: unsubscribe event
                // SimpleForecaster.OnPointForecasted -= 
            }

            var (rmse, nonpred) = MathExtended.CalculateRMSE(results, tmp.ToArray());
            logger.LogInformation("RMSE = {rmse}", rmse);
            logger.LogInformation("Non predicted points = {nonpred}", nonpred);
            writer.Write(results, DefaultParams.forecastPath);


            // Temporary stub
            //Span<float> tmp = series.Skip(series.Length - 1000).ToArray();

            //var next = SimpleForecaster.ForecastPoint(templates, clusters, tmp, args.Error, 10);
            //logger.LogInformation("Next point value = {next}", next);
#if DEBUG
            Console.ReadKey();
#endif

            return 0;
        }        
    }
}
