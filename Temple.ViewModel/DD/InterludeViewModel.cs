using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel.DD;

public class InterludeViewModel : ViewModelBase
{
    private readonly ApplicationController _controller;

    private string _text;

    public string Text
    {
        get => _text;
        set
        {
            if (_text == value) return;

            _text = value;
            RaisePropertyChanged();
        }
    }

    public RelayCommand ContinueCommand { get; }

    public InterludeViewModel(
        ApplicationController controller)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));

        ContinueCommand = new RelayCommand(_controller.ExitState);
    }
}