using System;
using CommandLine;
using System.IO;
using System.Linq;
using Tellure.Lib;
using System.Numerics;

namespace Tellure.CLI
{
    //TODO: add HelpText
    [Verb("generate", HelpText = "Generates chaotic series from Lorentz Equation")]
    class GenerateOptions
    {
        [Option('g', Default = 10f, HelpText = "")]
        public float Sigma { get; set; }
        [Option('r', Default = 28f, HelpText = "")]
        public float R { get; set; }
        [Option('b', Default = 8f/3, HelpText = "")]
        public float B { get; set; }
        [Option('s', Default = 0.05f, HelpText = "")]
        public float Step { get; set; }
        [Option('c', Default = 10000, HelpText = "")]
        public int Count { get; set; }

        [Option('o', HelpText = "")]
        public string OutFile { get; set; }

        [Option("normalize", Default = false, HelpText = "")]
        public bool Normalize { get; set; }
        [Option("print", Default = true, HelpText = "")]
        public bool Print { get; set; }
    }

    [Verb("normalize", HelpText = "Normalizes sequence that is readed from file, by default output range values would be [-1; 1]")]
    class NormalizeOptions
    {
        //TODO: add normalization TO and FROM, delimiters if file format is not strict
        [Option('f', Required = true, HelpText = "")]
        public string FileName { get; set; }
        [Option('o', HelpText = "")]
        public string OutFile { get; set; }
    }
    class Program
    {
        static int Main(string[] args)
        {
            //TODO: add logger
            return Parser.Default.ParseArguments<NormalizeOptions, GenerateOptions>(args)
                .MapResult(
                (GenerateOptions opts) => generate(opts),
                (NormalizeOptions opts) => normalize(opts),
                errs => 1);//TODO: handle errors

            int generate(GenerateOptions opts)
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

                if(!string.IsNullOrEmpty(opts.OutFile))
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
            int normalize(NormalizeOptions opts)
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
        }
    }
}
