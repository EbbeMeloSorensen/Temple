using GalaSoft.MvvmLight;

namespace Temple.UI.WPF
{
    public class SplashScreenViewModel : ViewModelBase
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

}
