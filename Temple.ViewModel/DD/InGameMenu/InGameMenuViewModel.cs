using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;
using Temple.ViewModel.DD.Quests;

namespace Temple.ViewModel.DD.InGameMenu;

public class InGameMenuViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private ApplicationStatePayload _payloadForNextState;

    public InventoryViewModel InventoryViewModel { get; } = new();
    public QuestCollectionViewModel QuestCollectionViewModel { get; }

    public RelayCommand Exit_Command { get; }

    public InGameMenuViewModel(
        ApplicationController controller,
        IQuestStatusReadModel questStatusReadModel)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        QuestCollectionViewModel = new QuestCollectionViewModel(
            questStatusReadModel,
            controller.EventBus);

        Exit_Command = new RelayCommand(() =>
        {
            _controller.GoToNextApplicationState(_payloadForNextState);
        });
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var inGameMenuPayload = payload as InGameMenuPayload
                                ?? throw new ArgumentException("Payload is not of type InGameMenuPayload", nameof(payload));

        _payloadForNextState = inGameMenuPayload.PayloadForNextState;

        return this;
    }
}

