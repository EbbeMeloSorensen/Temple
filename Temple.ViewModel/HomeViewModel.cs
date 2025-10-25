using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel
{
    public class HomeViewModel : ViewModelBase
    {
        private readonly ApplicationController _controller;

        public RelayCommand GoToSmurfManagementCommand { get; }
        public RelayCommand GoToPeopleManagementCommand { get; }
        public RelayCommand ExportStateMachineCommand { get; }
        public RelayCommand ShutdownCommand { get; }

        public HomeViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            GoToPeopleManagementCommand = new RelayCommand(_controller.GoToPeopleManagement);
            GoToSmurfManagementCommand = new RelayCommand(_controller.GoToSmurfManagement);
            ExportStateMachineCommand = new RelayCommand(_controller.ExportStateMachineAsGraph);
            ShutdownCommand = new RelayCommand(_controller.Shutdown);
        }
    }
}
