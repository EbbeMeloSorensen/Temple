using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

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
            throw new NotImplementedException();
        });

        TakeQuest_Command = new RelayCommand(() =>
        {
            throw new NotImplementedException();
        });
    }
}
