using System;
using CommandLine;
using System.IO;
using System.Linq;
using Tellure.Lib;

namespace Tellure.CLI
{
    [Verb("generate", HelpText = "Generates chaotic series from Lorentz Equation")]
    class GenerateOptions
    {
        //TODO: add generator options
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
                errs => 1);

            int generate(GenerateOptions opts)
            {
                //TODO: invoke generator
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
                //TODO: check for file format and use different formatters
                using (var writer = new StreamWriter(opts.OutFile))
                {
                    ServiceStack.Text.CsvSerializer.SerializeToWriter(normalized, writer);
                }

                return 0;
            }
        }
    }
}
