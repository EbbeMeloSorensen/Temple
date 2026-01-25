using Temple.Application.DD;

namespace Temple.Application.Interfaces;

public interface IDialogueSession
{
    string NPCPortraitPath { get; }

    string CurrentNPCText { get; }

    IReadOnlyList<DialogueChoice> AvailableChoices { get; }

    void SelectChoice(int choiceId);

    bool IsFinished { get; }
}