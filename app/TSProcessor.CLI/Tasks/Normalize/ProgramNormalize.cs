using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using Tellure.Generator;

namespace TSProcessor.CLI.Tasks.Normalize
{
    partial class Normalizer
    {
        public static int Normalize(NormalizeOptions opts, ILogger logger)
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
