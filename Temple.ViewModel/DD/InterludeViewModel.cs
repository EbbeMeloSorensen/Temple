using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State;

namespace Temple.ViewModel.DD;

public class InterludeViewModel : TempleViewModel
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

        //ContinueCommand = new RelayCommand(_controller.GoToBattle);
        ContinueCommand = new RelayCommand(_controller.GoToExploration);
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        switch (payload.JustAString)
        {
            case "Intro":
                Text = "Welcome to the adventure! Your journey begins now.";
                break;
            default:
                Text = "An unknown interlude has occurred.";
                break;
        }

        return this;
    }
}