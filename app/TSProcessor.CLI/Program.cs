using CommandLine;
using ILGPU;
using ILGPU.Runtime;
using ILGPU.Runtime.Cuda;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using TSProcessor.CLI.Tasks.Clusterize;
using TSProcessor.CLI.Tasks.Forecast;
using TSProcessor.CLI.Tasks.Generate;
using TSProcessor.CLI.Tasks.Normalize;
using TSProcessor.CLI.Tasks.Paint;

namespace TSProcessor.CLI
{
    partial class Program
    {
        static int Main(string[] args)
        {
            ServiceProvider serviceProvider = CreateServiceProvider();
            var logger = serviceProvider.GetService<ILogger<Program>>();
            var writer = serviceProvider.GetService<FileWriter>();

            return Parser.Default.ParseArguments<NormalizeOptions, GenerateOptions, PaintOptions, ClusterizationOptions, ForecastOptions>(args)
                .MapResult(
                (GenerateOptions opts) => Generator.Generate(opts, logger, writer),
                (NormalizeOptions opts) => Normalizer.Normalize(opts, logger),
                (PaintOptions opts) => Painter.Paint(opts, writer, logger),
                (ClusterizationOptions opts) => Clusterizer.Clusterize(opts, logger, writer),
                (ForecastOptions opts) => Forecaster.Forecast(opts, writer, logger),
                errs => HandleParseError(errs, logger));
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", false)
                .Build();
            serviceCollection.AddSingleton(configuration);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            serviceCollection.AddLogging(logging => logging.AddSerilog());
            serviceCollection.AddTransient<FileWriter, FileWriter>();
            serviceCollection.AddTransient<Context, Context>();
            serviceCollection.AddTransient<Accelerator, CudaAccelerator>();
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            return serviceProvider;
        }

        private static int HandleParseError(IEnumerable<Error> errs, Microsoft.Extensions.Logging.ILogger logger)
        {
            foreach (var error in errs)
            {
                logger.LogError("Error occured with parsing arguments. {error}", error);
            }
#if DEBUG
            Console.ReadKey();
#endif
            return 1;
        }
    }
}
