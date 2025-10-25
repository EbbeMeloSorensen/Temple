using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel.PR
{
    public class MainWindowViewModel_PR : ViewModelBase
    {
        private readonly ApplicationController _controller;

        public RelayCommand GoToHomeCommand{ get; }

        public MainWindowViewModel_PR(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            GoToHomeCommand = new RelayCommand(_controller.GoToHome);
        }
    }
}
