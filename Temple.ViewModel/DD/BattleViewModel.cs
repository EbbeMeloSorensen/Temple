using GalaSoft.MvvmLight;
using Temple.Application.Core;

namespace Temple.ViewModel.DD;

public class BattleViewModel : ViewModelBase
{
    private readonly ApplicationController _controller;

    public BattleViewModel(
        ApplicationController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }
}