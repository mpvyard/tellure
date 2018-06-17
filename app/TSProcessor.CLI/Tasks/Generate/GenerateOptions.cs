using CommandLine;

namespace TSProcessor.CLI.Tasks.Generate
{
    //TODO: add HelpText
    [Verb("generate", HelpText = "Generates chaotic series from Lorentz Equation")]
    class GenerateOptions
    {
        [Option('g', Default = DefaultParams.sigma, HelpText = "")]
        public float Sigma { get; set; }
        [Option('r', Default = DefaultParams.r, HelpText = "")]
        public float R { get; set; }
        [Option('b', Default = DefaultParams.b, HelpText = "")]
        public float B { get; set; }
        [Option('s', Default = DefaultParams.generationStep, HelpText = "")]
        public float Step { get; set; }
        [Option('p', Default = DefaultParams.skipCount, HelpText = "")]
        public int Skip { get; set; }
        [Option('c', Default = DefaultParams.sequenceCount, HelpText = "")]
        public int Count { get; set; }

        [Option('o', HelpText = "")]
        public string OutFile { get; set; }

        [Option("normalize", Default = false, HelpText = "")]
        public bool Normalize { get; set; }

        [Option('d', Default = 1)]
        public int Dimentions { get; set; }
    }
}
