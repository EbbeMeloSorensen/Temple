using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel.DD
{
    public class ExploreAreaViewModel
    {
        private readonly ApplicationController _controller;
        //private SceneViewController _sceneViewController;

        public RelayCommand ContinueCommand { get; }

        public ExploreAreaViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            ContinueCommand = new RelayCommand(_controller.ExitState);
        }
    }
}
