using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;
using Temple.Infrastructure.Presentation;
using Temple.ViewModel.DD.Exploration;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;

    public RelayCommand Leave_Command { get; }
    public RelayCommand TakeQuest_Command { get; }

    public DialogueViewModel(
        ApplicationController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

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
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var dialoguePayload = payload as DialoguePayload
                                 ?? throw new ArgumentException("Payload is not of type DialoguePayload", nameof(payload));

        if (dialoguePayload.DialogueId.Contains('_'))
        {
            var questId = dialoguePayload.DialogueId.Split('_')[1];
            var continueHere = 0;
        }
        else
        {
            throw new InvalidOperationException("We expect the Dialog id to include a quest id");
        }

        return this;
    }
}
