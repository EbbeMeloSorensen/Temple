using Stateless;
using Temple.Application.State.OldPrinciple;

namespace Temple.Application.Interfaces;

public interface IStateMachineIO
{
    void ExportTheDamnThing(
        StateMachine<ApplicationState, ApplicationTrigger> stateMachine);
}