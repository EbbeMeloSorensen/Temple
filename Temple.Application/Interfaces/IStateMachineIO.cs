using Stateless;
using Temple.Application.State.NewPrinciple;

namespace Temple.Application.Interfaces;

public interface IStateMachineIO
{
    void ExportTheDamnThing(
        StateMachine<GameScene, Trigger> stateMachine);
}