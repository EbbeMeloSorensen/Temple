using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD;

public class InterludeViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private ApplicationStatePayload _payloadForNextState;

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

        ContinueCommand = new RelayCommand(() =>
        {
            _controller.GoToNextApplicationState(_payloadForNextState);
        });
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var interludePayload = payload as InterludePayload
            ?? throw new ArgumentException("Payload is not of type InterludePayload", nameof(payload));

        Text = interludePayload.Text;

        _payloadForNextState = interludePayload.PayloadForNextState;

        return this;
    }
}