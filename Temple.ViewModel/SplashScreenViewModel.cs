using GalaSoft.MvvmLight;

namespace Temple.ViewModel
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
