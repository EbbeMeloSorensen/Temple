using Temple.Application.DD;

namespace Temple.Application.Interfaces;

public interface IDialogueSession
{
    string CurrentNPCText { get; }

    IReadOnlyList<DialogueChoice> AvailableChoices { get; }

    void SelectChoice(int choiceId);

    bool IsFinished { get; }
}