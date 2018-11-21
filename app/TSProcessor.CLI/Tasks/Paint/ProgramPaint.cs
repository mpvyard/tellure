using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tellure.Algorithms;
using Tellure.Algorithms.Painting;
using TSProcessor.CLI.IO;

namespace TSProcessor.CLI.Tasks.Paint
{
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
                    var template = Template.Parse(file);
                    var templateClusters = ServiceStack.Text.JsonSerializer.DeserializeFromReader<float[][]>(stream);
                    templates.Add(template);
                    clusters.Add(templateClusters);
                }
            }
            //int[] result = Process(templates, clusters, series, args.Error);

            int[] result = AcceleratorPainter.Paint(templates, clusters, series, args.Error);

            //int[] result = CPUPainter.Paint(templates, clusters, series, args.Error).ToArray();
            writer.Write(result, DefaultParams.paintPath);
            return 0;
        }
    }
}
