using Stateless;
using Stateless.Graph;
using Temple.Application.Interfaces;
using Temple.Application.State.OldPrinciple;

namespace Temple.Infrastructure.IO
{
    public class StateMachineIO : IStateMachineIO
    {
        public void ExportTheDamnThing(
            StateMachine<ApplicationState, ApplicationTrigger> stateMachine)
        {
            var dot = UmlDotGraph.Format(stateMachine.GetInfo());

            using var outputFile = new StreamWriter(Path.Combine(@"C:\Temp", "StateMachine.dot"));
            outputFile.WriteLine(dot);
        }
    }
}
