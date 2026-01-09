using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.InGameMenu;

public class InGameMenuViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;

    public RelayCommand Exit_Command { get; }

    public InGameMenuViewModel(
        ApplicationController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        Exit_Command = new RelayCommand(() =>
        {
            _controller.GoToNextApplicationState(new ExplorationPayload
            {
                Site = _controller.ApplicationData.CurrentSite
            });
        });
    }
}

