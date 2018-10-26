﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSProcessor.CLI.Tasks.Forecast
{
    [Verb("forecast")]
    class ForecastOptions
    {
        [Option('s', HelpText = "")]
        public string SeriesFileName { get; set; }
        [Option('c', HelpText = "")]
        public string ClustersDirectory { get; set; }
        [Option('e', Default = 0.1f)]
        public float Error { get; set; }
        [Option('f', Default = 10)]
        public int ForecastringSteps { get; set; }
    }
}
