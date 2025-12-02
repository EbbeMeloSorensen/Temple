using Stateless;
using Stateless.Graph;
using Temple.Application.Interfaces;
using Temple.Application.State;

namespace Temple.Infrastructure.IO
{
    public class StateMachineIO : IStateMachineIO
    {
        public void ExportTheDamnThing(
            StateMachine<ApplicationStateType, Trigger> stateMachine)
        {
            var dot = UmlDotGraph.Format(stateMachine.GetInfo());

            using var outputFile = new StreamWriter(Path.Combine(@"C:\Temp", "StateMachine2.dot"));
            outputFile.WriteLine(dot);
        }
    }
}
