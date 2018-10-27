using CommandLine;

namespace TSProcessor.CLI.Tasks.Generate
{
    [Verb("generate", HelpText = "Generates chaotic series from Lorentz Equation")]
    class GenerateOptions
    {
        [Option('g', Default = DefaultParams.sigma, HelpText = "\u03A3 - Parameter of Lorenz system, Prandtl number")]
        public float Sigma { get; set; }
        [Option('r', Default = DefaultParams.r, HelpText = "\u03C1 - Parameter of Lorenz system, Rayleigh number")]
        public float R { get; set; }
        [Option('b', Default = DefaultParams.b, HelpText = "\u03B2 - Parameter of Lorenz system, certain physical dimensions of the layer")]
        public float B { get; set; }
        [Option('s', Default = DefaultParams.generationStep, HelpText = "h - step of integration by RG4 method (the Runge-Kutta method)")]
        public float Step { get; set; }

        [Option('o', HelpText = "Path to the file to save generated sequence")]
        public string OutFile { get; set; }
        [Option('f', HelpText = "Path to the file to save generated test sequence")]
        public string OutTestsFile { get; set; }

        [Option('c', Default = DefaultParams.sequenceCount, HelpText = "The length of generated sequence")]
        public int DataCount { get; set; }
        [Option('t', Default = DefaultParams.testsCount, HelpText = "The length of generated testing sequence")]
        public int TestsCount { get; set; }

        [Option("normalize", Default = false, HelpText = "The option to normalize sequence or not")]
        public bool Normalize { get; set; }

        // TODO: add possibility to specify dimentions by the flags
        // example: tsprocessor -y | tsprocessor -xyz
        [Option('d', HelpText = "The number of dimmentions in the output sequence", Default = 1)]
        public int Dimentions { get; set; }

        // TODO: add usage examples
    }
}
