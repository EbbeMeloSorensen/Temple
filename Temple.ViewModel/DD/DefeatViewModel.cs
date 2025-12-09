using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel.DD
{
    public class DefeatViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;

        public RelayCommand ContinueCommand { get; }

        public DefeatViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            ContinueCommand = new RelayCommand(_controller.ExitState);
        }
    }
}
