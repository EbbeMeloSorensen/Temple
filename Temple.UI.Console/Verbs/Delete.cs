﻿using CommandLine;

namespace Temple.UI.Console.Verbs
{
    [Verb("delete", HelpText = "Delete an existing person.")]
    public sealed class Delete
    {
        [Option('i', "id", Required = true, HelpText = "Person ID")]
        public string ID { get; set; }
    }
}
