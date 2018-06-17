using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Tellure.Generator;

namespace TSProcessor.CLI.Tasks.Generate
{
    static class Generator
    {
        public static int Generate(GenerateOptions opts, ILogger logger, FileWriter writer)
        {
            //TODO: add checks of opts
            opts.OutFile = opts.OutFile ?? DefaultParams.seriesPath;

            logger.LogInformation("Operation started...");

            logger.LogInformation("Generate time-series");

            var generator = new TimeSeriesGenerator(opts.Sigma, opts.R, opts.B);
            var sequence = generator.Generate(DefaultParams.Y0, opts.Step, opts.Skip + opts.Count)
                .Skip(opts.Skip);

            if (opts.Dimentions == 1)
            {
                Get1dSequence(opts, logger, writer, sequence);
            }

            if (opts.Dimentions == 3)
            {
                Get3dSequence(opts, logger, writer, sequence);
            }

            //TODO: add write to MongoDb
            logger.LogInformation("Operation completed");
#if DEBUG
            Console.ReadLine();
#endif
            return 0;
        }

        private static void Get3dSequence(GenerateOptions opts, ILogger logger, FileWriter writer, IEnumerable<Vector3> sequence)
        {
            logger.LogInformation("Normalize generated series");

            if (opts.Normalize)
            {
                sequence = sequence.Normalize();
            }
            logger.LogDebug("Sequence: {sequence}",
                ServiceStack.Text.CsvSerializer.SerializeToString(sequence));

            logger.LogInformation("Writing to file {file} started", opts.OutFile);
            writer.Write(sequence, opts.OutFile);
            logger.LogInformation("Writing to file {file} finished", opts.OutFile);
        }

        private static void Get1dSequence(GenerateOptions opts, ILogger logger, FileWriter writer, IEnumerable<Vector3> sequence)
        {
            logger.LogInformation("Normalize generated series");

            var sequenceX = sequence.Select(number => number.X);
            if (opts.Normalize)
            {
                sequenceX = sequenceX.Normalize();
            }

            logger.LogDebug("Sequence: {sequence}",
                ServiceStack.Text.CsvSerializer.SerializeToString(sequenceX));

            logger.LogInformation("Writing to file {file} started", opts.OutFile);
            writer.Write(sequenceX, opts.OutFile);
            logger.LogInformation("Writing to file {file} finished", opts.OutFile);
        }
    }
}
