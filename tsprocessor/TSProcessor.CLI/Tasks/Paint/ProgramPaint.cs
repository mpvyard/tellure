using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TSProcessor.CLI.Tasks.Paint
{
    static class Painter
    {
        public static int Paint(PaintOptions args, ILogger logger)
        {
            if (!File.Exists(args.SeriesFileName))
            {
                return 1;
            }

            if (!Directory.Exists(args.ClustersDirectory))
            {
                return 1;
            }
            //TODO: read series

            //TODO: read clusters

            //TODO: use Painter.cs to calculate heatmaps

            //TODO: write results out
            return 0;
        }
    }
}
