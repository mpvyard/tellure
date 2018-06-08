using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSProcessor.CLI.Tasks.Paint
{
    [Verb("paint")]
    class PaintOptions
    {
        [Option('s', HelpText = "")]
        public string SeriesFileName { get; set; }
        [Option('c', HelpText = "")]
        public string ClustersDirectory { get; set; }
        [Option('e', Default = 0.01)]
        public double Error { get; set; }
    }
}
