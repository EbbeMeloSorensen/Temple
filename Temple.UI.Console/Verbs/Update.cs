using CommandLine;

namespace Temple.UI.Console.Verbs
{
    [Verb("updateperson", HelpText = "Update an existing person.")]
    public sealed class Update
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }

        [Option('f', "firstname", Required = false, HelpText = "First Name")]
        public string FirstName { get; set; }

        [Option('t', "timeofchange", Required = false, HelpText = "Time of change", Default = "")]
        public string TimeOfChange { get; set; }
    }
}
