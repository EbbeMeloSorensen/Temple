using CommandLine;

namespace Temple.UI.Console.Verbs
{
    [Verb("details", HelpText = "Get person details.")]
    public sealed class Details
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }

        [Option('d', "databasetime", Required = false, HelpText = "Database Time", Default = "")]
        public string DatabaseTime { get; set; }

        [Option('f', "writetofile", Required = false, HelpText = "Write to file instead of console", Default = false)]
        public bool WriteToFile { get; set; }
    }
}
