using GalaSoft.MvvmLight;
using System.ComponentModel;

namespace Temple.POC.State.WPFApp;

public class SplashViewModel : ViewModelBase
{
    private string _statusMessage = "Starting application...";

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage == value) return;

            _statusMessage = value;
            RaisePropertyChanged();
        }
    }
}

