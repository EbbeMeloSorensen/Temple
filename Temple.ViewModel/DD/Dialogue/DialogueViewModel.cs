using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.ViewModel.DD.Quests;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private readonly QuestStatusReadModel _questStatusReadModel;

    public bool TakeQuestPossible { get; }

    public RelayCommand Leave_Command { get; }
    public RelayCommand TakeQuest_Command { get; }

    public DialogueViewModel(
        ApplicationController controller,
        QuestStatusReadModel questStatusReadModel)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _questStatusReadModel = questStatusReadModel ?? throw new ArgumentNullException(nameof(questStatusReadModel));

        Leave_Command = new RelayCommand(() =>
        {
            _controller.GoToNextApplicationState(new ExplorationPayload
            {
                Site = _controller.ApplicationData.CurrentSite 
            });
        });

        TakeQuest_Command = new RelayCommand(() =>
        {
            throw new NotImplementedException();
        });

        TakeQuestPossible = false;
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var dialoguePayload = payload as DialoguePayload
                                 ?? throw new ArgumentException("Payload is not of type DialoguePayload", nameof(payload));

        // På sigt skal det være sådan at det at bevæge sig ned i et dialog tree kan gøre at visse quests gøres tilgængeligt (available),
        // og i øvrigt at tilgængelige quests kan gøres aktive.
        // Til en start gør vi dog bare det at den ene quest, der er i spillet bliver tilgængelig, og så skal vi abonnere på at den faktisk
        // bliver det, så vi kan sætte TakeQuestPossible til true og dermed vise knappen i UI'et.

        _controller.EventBus.Publish(new DialogueEvent("mayor"));

        return this;
    }
}
