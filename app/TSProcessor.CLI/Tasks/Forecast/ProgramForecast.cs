using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
            var results = SimpleForecaster.Forecast(templates, clusters, series, args.Error);
            var (rmse, nonpred) = SimpleForecaster.CalculateRMSE(results, series);
            logger.LogInformation("RMSE = {rmse}", rmse);
            logger.LogInformation("Non predicted points = {nonpred}", nonpred);
            writer.Write(results, DefaultParams.forecastPath);
            return 0;
        }        
    }
}
