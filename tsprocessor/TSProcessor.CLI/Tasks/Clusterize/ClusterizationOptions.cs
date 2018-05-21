using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace TSProcessor.CLI.Tasks.Clusterize
{
    [Verb("clusterize")]
    class ClusterizationOptions
    {
        [Option(Default = new int[] { 1, 1, 1, 1}, Separator = '.')]
        public int[] From { get; set; }
        [Option(Default = new int[] { 10, 10, 10, 10}, Separator = '.')]
        public int[] To { get; set; }
    }
}
