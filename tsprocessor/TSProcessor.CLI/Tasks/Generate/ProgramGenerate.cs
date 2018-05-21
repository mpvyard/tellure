using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using Tellure.Lib;

namespace TSProcessor.CLI.Tasks.Generate
{
    static class Generator
    {
        public static int Generate(GenerateOptions opts, ILogger logger)
        {
            //TODO: add checks of opts
            var generator = new TimeSeriesGenerator(opts.Sigma, opts.R, opts.B);
            var y0 = new Vector3(10, 10, 10);
            var sequence = generator.Generate(y0, opts.Step, opts.Skip, opts.Count);
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
    }
}
