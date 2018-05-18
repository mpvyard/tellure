using CommandLine;

namespace TSProcessor.CLI.Options
{
    //TODO: add HelpText
    [Verb("normalize", HelpText = "Normalizes sequence that is readed from file, by default output range values would be [-1; 1]")]
    class NormalizeOptions
    {
        //TODO: add normalization TO and FROM, delimiters if file format is not strict
        [Option('f', Required = true, HelpText = "")]
        public string FileName { get; set; }
        [Option('o', HelpText = "")]
        public string OutFile { get; set; }
    }
}
