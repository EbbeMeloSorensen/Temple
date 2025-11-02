using Temple.ViewModel.DD.Battle.BusinessLogic;

namespace Temple.ViewModel.DD.BusinessLogic.Complex
{
    // The NoEvent class is used by the engine to inform its host that even though the engine passes control to the host,
    // the host should pass back control to the engine immediately by calling the main loop that e.g. includes ExecuteNextAction
    public class NoEvent : IBattleEvent
    {
    }
}
