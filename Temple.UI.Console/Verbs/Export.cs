using CommandLine;

namespace Temple.UI.Console.Verbs
{
    [Verb("export", HelpText = "Export all people.")]
    public sealed class Export
    {
        [Option('f', "filename", Required = true, HelpText = "File Name")]
        public string FileName { get; set; }
    }
}