using Stateless;
using Stateless.Graph;
using System.Text.RegularExpressions;
using Temple.Application.Interfaces;
using Temple.Application.State;

namespace Temple.Infrastructure.IO
{
    public class StateMachineIO : IStateMachineIO
    {
        public void ExportTheDamnThing(
            StateMachine<StateMachineState, ApplicationStateShiftTrigger> stateMachine)
        {
            var raw = UmlDotGraph.Format(stateMachine.GetInfo());

            var cleaned = Regex.Replace(
                raw,
                // Capture the opening label=", then capture any text up to (but not including) the first '|entry/|exit'
                // If no entry/exit exists, the whole label is preserved.
                @"(label="")([^""]*?)(?:\|(?:entry|exit)\s*/\s*[^""]*)("")",
                "$1$2$3",
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            );
            
            using var outputFile = new StreamWriter(Path.Combine(@"C:\Temp", "StateMachine2.dot"));
            outputFile.WriteLine(cleaned);
        }
    }
}
