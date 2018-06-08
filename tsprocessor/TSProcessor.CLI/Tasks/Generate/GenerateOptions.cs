using CommandLine;
using System;
using System.IO;

namespace TSProcessor.CLI.Tasks.Generate
{
    //TODO: add HelpText
    [Verb("generate", HelpText = "Generates chaotic series from Lorentz Equation")]
    class GenerateOptions
    {
        [Option('g', Default = 10f, HelpText = "")]
        public float Sigma { get; set; }
        [Option('r', Default = 28f, HelpText = "")]
        public float R { get; set; }
        [Option('b', Default = 8f/3, HelpText = "")]
        public float B { get; set; }
        [Option('s', Default = 0.05f, HelpText = "")]
        public float Step { get; set; }
        [Option('p', Default = 3000, HelpText = "")]
        public int Skip { get; set; }
        [Option('c', Default = 13500, HelpText = "")]
        public int Count { get; set; }

        [Option('o', HelpText = "")]
        public string OutFile { get; set; }

        [Option("normalize", Default = false, HelpText = "")]
        public bool Normalize { get; set; }

        [Option(Default = 1)]
        public int Dimentions { get; set; }
    }
}
