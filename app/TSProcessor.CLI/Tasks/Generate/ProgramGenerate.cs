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
        // TODO: rework code from using 1d and 3d methods to just one
        public static int Generate(GenerateOptions opts, ILogger logger, FileWriter writer)
        {
            //TODO: add checks of opts
            opts.OutFile = opts.OutFile ?? DefaultParams.seriesPath;
            opts.OutTestsFile = opts.OutTestsFile ?? DefaultParams.testsPath;

            logger.LogInformation("Operation started...");

            logger.LogInformation("Generate time-series");
            var rnd = new Random();
            int distance = rnd.Next(opts.DataCount / 10);

            var generator = new TimeSeriesGenerator(opts.Sigma, opts.R, opts.B);

            var sequence = generator.Generate(DefaultParams.Y0, opts.Step, DefaultParams.skipCount + opts.DataCount);
            sequence = sequence.Skip(DefaultParams.skipCount);

            var testSequence = generator.Generate(DefaultParams.Y0, opts.Step,
                DefaultParams.skipCount + opts.DataCount + distance + opts.TestsCount);

            if (opts.Dimentions == 1)
            {
                Get1dSequence(opts, logger, writer, sequence, testSequence);
            }

            if (opts.Dimentions == 3)
            {
                Get3dSequence(opts, logger, writer, sequence, testSequence);
            }
            //TODO: add write to MongoDb
            logger.LogInformation("The distance between learning sequence and testing sequence = {distance}", distance);
            logger.LogInformation("Operation completed");
#if DEBUG
            Console.ReadLine();
#endif
            return 0;
        }

        private static void Get3dSequence(GenerateOptions opts, ILogger logger, FileWriter writer, IEnumerable<Vector3> sequence, IEnumerable<Vector3> testSequence)
        {
            if (opts.Normalize)
            {
                logger.LogInformation("Normalize generated series");

                var sec = sequence.Concat(testSequence);
                sec = sec.Normalize();
                sequence = sec.Take(sequence.Count());
                testSequence = sec.Skip(sequence.Count());
            }

            var arrSequence = sequence.Select(vec => new float[] { vec.X, vec.Y, vec.Z });
            var arrTestSequence = testSequence.Select(vec => new float[] { vec.X, vec.Y, vec.Z });

            writer.Write(arrSequence, opts.OutFile);
            logger.LogDebug("Writing sequence to {file}", opts.OutFile);
            logger.LogInformation("Writing sequence to file finished");

            writer.Write(arrTestSequence, opts.OutTestsFile);
            logger.LogDebug("Writing testing sequence to {file}", opts.OutTestsFile);
            logger.LogInformation("Writing testing sequence to file finished");
        }

        private static void Get1dSequence(GenerateOptions opts, ILogger logger, FileWriter writer, IEnumerable<Vector3> sequence, IEnumerable<Vector3> testSequence)
        {
            var sequenceX = sequence.Select(number => number.X);
            var testSequenceX = testSequence.Select(number => number.X);

            if (opts.Normalize)
            {
                logger.LogInformation("Normalize generated series");

                var sec = sequenceX.Concat(testSequenceX);
                sec = sec.Normalize();
                sequenceX = sec.Take(sequence.Count());
                testSequenceX = sec.Skip(sequence.Count());
            }

            writer.Write(sequenceX, opts.OutFile);
            logger.LogDebug("Writing sequence to {file}", opts.OutFile);
            logger.LogInformation("Writing sequence to file finished");

            writer.Write(testSequenceX, opts.OutTestsFile);
            logger.LogDebug("Writing testing sequence to {file}", opts.OutTestsFile);
            logger.LogInformation("Writing testing sequence to file finished");
        }
    }
}
