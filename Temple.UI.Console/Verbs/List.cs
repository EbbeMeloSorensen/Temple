using CommandLine;

namespace Temple.UI.Console.Verbs
{
    [Verb("list", HelpText = "List all people.")]
    public sealed class List
    {
        [Option('h', "historicaltime", Required = false, HelpText = "Historical Time", Default = "")]
        public string HistoricalTime { get; set; }

        [Option('d', "databasetime", Required = false, HelpText = "Database Time", Default = "")]
        public string DatabaseTime { get; set; }

        [Option('x', "excludecurrentobjects", Required = false, HelpText = "Exclude Current Objects", Default = false)]
        public bool ExcludeCurrentObjects { get; set; }

        [Option('i', "includehistoricalobjects", Required = false, HelpText = "Include Historical Objects", Default = false)]
        public bool IncludeHistoricalObjects { get; set; }

        [Option('f', "writetofile", Required = false, HelpText = "Write to file instead of console", Default = false)]
        public bool WriteToFile { get; set; }
    }
}
