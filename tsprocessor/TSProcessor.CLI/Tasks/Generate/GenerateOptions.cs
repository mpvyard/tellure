using CommandLine;

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
        [Option('c', Default = 10000, HelpText = "")]
        public int Count { get; set; }

        [Option('o', HelpText = "")]
        public string OutFile { get; set; }

        [Option("normalize", Default = false, HelpText = "")]
        public bool Normalize { get; set; }
        [Option("print", Default = true, HelpText = "")]
        public bool Print { get; set; }
    }
}
