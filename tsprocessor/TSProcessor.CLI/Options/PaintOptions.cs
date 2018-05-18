using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSProcessor.CLI.Options
{
    [Verb("paint")]
    class PaintOptions
    {
        [Option('s', Required = true, HelpText = "")]
        public string SeriesFileName { get; set; }
        [Option('c', Required = true, HelpText = "")]
        public string ClustersDirectory { get; set; }
        [Option('e', Default = 0.01)]
        public double Error { get; set; }
    }
}
