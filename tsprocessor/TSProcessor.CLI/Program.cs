using System;
using CommandLine;
using System.IO;
using System.Linq;
using Tellure.Lib;
using System.Numerics;
using TSProcessor.CLI.Options;
using System.Collections.Generic;
using Riowil.Lib;
using Riowil.Entities;
using System.Numerics;

namespace TSProcessor.CLI
{
    class Program
    {
        static int Main(string[] args)
        {
            //TODO: add logger
            return Parser.Default.ParseArguments<NormalizeOptions, GenerateOptions, PaintOptions, ClusterizationOptions>(args)
                .MapResult(
                (GenerateOptions opts) => Generate(opts),
                (NormalizeOptions opts) => Normalize(opts),
                (PaintOptions opts) => Paint(opts),
                (ClusterizationOptions opts) => Clusterize(opts),
                errs => HandleParseError(errs));
        }

        private static int Generate(GenerateOptions opts)
        {
            //TODO: add checks of opts
            var generator = new TimeSeriesGenerator(opts.Sigma, opts.R, opts.B);
            var y0 = new Vector3(10, 10, 10);
            var sequence = generator.Generate(y0, opts.Step, opts.Count);
            //var sequenceX = sequence.Select(number => number.X);
            if (opts.Normalize)
            {
                sequence = sequence.Normalize();
            }

            if (!string.IsNullOrEmpty(opts.OutFile))
            {
                //TODO: check for file format and use different formatters
                using (var writer = new StreamWriter(opts.OutFile))
                {
                    ServiceStack.Text.CsvSerializer.SerializeToWriter(sequence, writer);
                }
            }

            if (opts.Print)
            {
                var output = ServiceStack.Text.CsvSerializer.SerializeToString(sequence);
                Console.WriteLine(output);
            }
            //TODO: add write to MongoDb
#if DEBUG
            Console.ReadLine();
#endif
            return 0;
        }
        private static int Normalize(NormalizeOptions opts)
        {
            //TODO: add checks of opts
            //TODO: use deserializer
            //TODO: use Span<T>, if it would be possible to serrialize it
            var series = File.ReadAllText(opts.FileName)
                .Split(';')
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => double.Parse(x));

            var normalized = series.Normalize();
            if (!string.IsNullOrEmpty(opts.OutFile))
            {
                //TODO: check for file format and use different formatters
                using (var writer = new StreamWriter(opts.OutFile))
                {
                    ServiceStack.Text.CsvSerializer.SerializeToWriter(normalized, writer);
                }
            }
            return 0;
        }

        private static int Clusterize(ClusterizationOptions opts)
        {
            throw new NotImplementedException();
            SeriesParams seriesParams = GetCurrentParams();
            Vector3 Y02 = new Vector3 (10, -1, 1);
            TimeSeriesGenerator lr = new TimeSeriesGenerator(10,280,2.666f);
            IEnumerable<Vector3> sequence = lr.Generate(Y02, 0.05f, seriesParams.Count);
            IEnumerable<Vector3> NormalizedSequence = SeriesNormalizer.Normalize(sequence);
            
            List<double> dots = new List<double>();

            foreach(Vector3 var in NormalizedSequence)
            {
                dots.Add(var.X);
                dots.Add(var.Y);
                dots.Add(var.Z);
            }
            Series series = new Series(seriesParams, dots);
            ClusterizeAll(series, seriesParams);
        }
        private static void ClusterizeAll(Series series, SeriesParams seriesParams)
        {
            int[] step = new int[4];
            List<ZVector> zVectors = new List<ZVector>();
            for (int a = 1; a <= 10; a++)
            {
                for (int b = 1; b <= 10; b++)
                {
                    for (int c = 1; c <= 10; c++)
                    {
                        for (int d = 1; d <= 10; d++)
                        {
                            step[0] = a;
                            step[1] = b;
                            step[2] = c;
                            step[3] = d;
                            zVectors.AddRange(FindZVectors(series, step, zVectors.Count));
                            Clusterize(seriesParams, zVectors);
                            zVectors.Clear();
                        }
                    }
                }
            }
        }
        private static void Clusterize(SeriesParams seriesParams, List<ZVector> zVectors)
        {
            IClusterizeAlgor algor = new WishartAlgor(new WishartParams { H = 0.2, K = 11 });
            List<InitialCluster> clusters = algor.Clusterize(zVectors);
        }
        private static SeriesParams GetCurrentParams()
        {
            SeriesType seriesType = SeriesType.Lorence;
            return new SeriesParams
            {
                Category = "",
                FirstInTraining = 0,
                CountInTraining = 10000,
                FirstInTest = 7000,
                CountInTest = 3000,
                FistInCheck = 23000,
                CountInCheck = 500,
                Type = seriesType
            };
        }
        private static IEnumerable<ZVector> FindZVectors(Series series, int[] step, int firstNumber)
        {
            List<double> teachSeries = series.GetTeachSeries();
            IEnumerable<ZVector> zVectors = ZVectorBuilder.Build(teachSeries, step, firstNumber);
            return zVectors;
        }

        private static int HandleParseError(IEnumerable<CommandLine.Error> errs)
        {
            //TODO: add handling
            //throw new NotImplementedException();
            return 1;
        }

        private static int Paint(PaintOptions args)
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

        static double[] ReadSeqence(string path)
        {
            return ServiceStack.Text.CsvSerializer.DeserializeFromString<double[]>(File.ReadAllText(path).Replace(',', '\n'));
        }

        static void WriteClusters(string path, double[,] clusters)
        {
            using (var writer = new StreamWriter(path))
            {
                //ServiceStack.Text.CsvSerializer.SerializeToWriter<double[,]>(clusters, writer);

                ServiceStack.Text.JsonSerializer.SerializeToWriter<double[,]>(clusters, writer);

                //var s = ServiceStack.Text.CsvSerializer.SerializeToString<double[,]>(clusters);
                //writer.Write(s);

                //ServiceStack.Text.CsvSerializer.WriteLateBoundObject(writer, clusters);  
            }

        }


    }
}
