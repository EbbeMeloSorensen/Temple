using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.ViewModel.DD.Quests;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private readonly QuestStatusReadModel _questStatusReadModel;
    private string _imagePath;
    private bool _takeQuestPossible;

    public string ImagePath
    {
        get { return _imagePath; }
        set
        {
            _imagePath = value;
            RaisePropertyChanged();
        }
    }

    public bool TakeQuestPossible
    {
        get => _takeQuestPossible;
        private set
        {
            _takeQuestPossible = value;
            RaisePropertyChanged();
        }
    }

    public RelayCommand Leave_Command { get; }
    public RelayCommand TakeQuest_Command { get; }

    public DialogueViewModel(
        ApplicationController controller,
        QuestStatusReadModel questStatusReadModel)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _questStatusReadModel = questStatusReadModel ?? throw new ArgumentNullException(nameof(questStatusReadModel));

        _questStatusReadModel.QuestStatusChanged += HandleQuestStatusChanged;

        Leave_Command = new RelayCommand(() =>
        {
            _controller.GoToNextApplicationState(new ExplorationPayload
            {
                Site = _controller.ApplicationData.CurrentSite 
            });
        });

        TakeQuest_Command = new RelayCommand(() =>
        {
            _controller.EventBus.Publish(new QuestAcceptedEvent("bandit_trouble"));
        });

        TakeQuestPossible = false;
    }

    private void HandleQuestStatusChanged(
        object? sender,
        QuestStatusChangedEventArgs e)
    {
        if (e.QuestId == "bandit_trouble")
        {
            TakeQuestPossible = e.QuestState == QuestState.Available;
        }
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var dialoguePayload = payload as DialoguePayload
                                 ?? throw new ArgumentException("Payload is not of type DialoguePayload", nameof(payload));

        ImagePath = "DD/Images/Innkeeper.png";

        // På sigt skal det være sådan at det at bevæge sig ned i et dialog tree kan gøre at visse quests gøres tilgængeligt (available),
        // og i øvrigt at tilgængelige quests kan gøres aktive.
        // Til en start gør vi dog bare det at den ene quest, der er i spillet bliver tilgængelig, og så skal vi abonnere på at den faktisk
        // bliver det, så vi kan sætte TakeQuestPossible til true og dermed vise knappen i UI'et.

        _controller.EventBus.Publish(new DialogueEvent("mayor"));

        return this;
    }

    public override void Cleanup()
    {
        // Unsubscribe to prevent memory leaks
        _questStatusReadModel.QuestStatusChanged -= HandleQuestStatusChanged;
        base.Cleanup();
    }
}
