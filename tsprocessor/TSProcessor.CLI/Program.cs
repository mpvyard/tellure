using System;
using CommandLine;
using System.IO;
using System.Linq;
using Tellure.Lib;
using System.Numerics;
using TSProcessor.CLI.Options;
using System.Collections.Generic;

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
                (ClusterizationOptions opts) => Cluserize(opts),
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

        private static int Cluserize(ClusterizationOptions opts)
        {
            throw new NotImplementedException();
        }

        private static int HandleParseError(IEnumerable<Error> errs)
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
