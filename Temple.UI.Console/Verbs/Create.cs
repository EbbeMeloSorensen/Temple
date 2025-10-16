using CommandLine;

namespace Temple.UI.Console.Verbs
{
    [Verb("create", HelpText = "Create a new Person.")]
    public sealed class Create
    {
        [Option('f', "firstname", Required = true, HelpText = "First Name")]
        public string FirstName { get; set; }

        [Option('s', "starttime", Required = false, HelpText = "Start Time", Default = "")]
        public string StartTime { get; set; }

        [Option('e', "endtime", Required = false, HelpText = "End Time", Default = "")]
        public string EndTime { get; set; }
    }
}
