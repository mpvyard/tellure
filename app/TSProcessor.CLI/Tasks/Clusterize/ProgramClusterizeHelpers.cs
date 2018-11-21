using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Tellure.Algorithms;
using Tellure.Entities;
using Tellure.Entities.Clusters;

namespace TSProcessor.CLI.Tasks.Clusterize
{
    static partial class Clusterizer
    {
        [Obsolete]
        private static void ClusterizeAll(IReadOnlyList<Vector4> series, int[] from, int[] to, ILogger logger)
        {
            foreach (var template in Wishart.GenerateTemplateForWishart(from, to))
            {
                var vectors = ZVectorBuilder.Build(series, template, 0);
                var clusters = Clusterize(vectors);
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                    "clusters", $"{template[0]}-{template[1]}-{template[2]}-{template[3]}.json");
                using (var writer = new StreamWriter(path))
                {
                    ServiceStack.Text.JsonSerializer.SerializeToWriter(clusters, writer);
                }
            }
        }

        private static IReadOnlyList<InitialCluster> Clusterize(IReadOnlyList<ZVector> zVectors)
        {
            WishartAlgor algor = new WishartAlgor(new WishartParams { H = 0.2, K = 11 });
            var clusters = algor.Clusterize(zVectors);
            return clusters;
        }
    }
}
