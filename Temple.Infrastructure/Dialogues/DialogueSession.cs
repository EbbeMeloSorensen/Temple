using Temple.Application.DD;
using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSession : IDialogueSession
{
    public string CurrentNPCText { get; }
    public IReadOnlyList<DialogueChoice> AvailableChoices { get; }
    public void SelectChoice(int choiceId)
    {
        throw new NotImplementedException();
    }

    public bool IsFinished { get; }
}