using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.InGameMenu;

public class InGameMenuViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private ApplicationStatePayload _payloadForNextState;

    public RelayCommand Exit_Command { get; }

    public InGameMenuViewModel(
        ApplicationController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        Exit_Command = new RelayCommand(() =>
        {
            //_controller.GoToWilderness();

            //_controller.GoToNextApplicationState(new ExplorationPayload
            //{
            //    Site = _controller.ApplicationData.CurrentSite
            //});

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

