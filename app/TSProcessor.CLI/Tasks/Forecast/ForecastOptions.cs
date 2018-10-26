using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSProcessor.CLI.Tasks.Forecast
{
    [Verb("forecast")]
    class ForecastOptions
    {
        [Option('s', HelpText = "The path to series data that should be forecasted")]
        public string SeriesFileName { get; set; }
        [Option('c', HelpText = "The path to the directory with clusters (model) for forecasting")]
        public string ClustersDirectory { get; set; }
        [Option('e', HelpText = "\u03B5 - forecasting parameter, representing the range of model to fit and be used to forecast", Default = 0.1f)]
        public float Error { get; set; }
        [Option('f', HelpText = "Forecasting parameter that represets - how many points after known series should forecast be done", Default = 10)]
        public int ForecastringSteps { get; set; }
    }
}
