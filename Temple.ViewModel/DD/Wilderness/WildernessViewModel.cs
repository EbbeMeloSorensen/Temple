using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.Wilderness
{
    public class WildernessViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;

        public RelayCommand GoToSite_Mine_Command { get; }
        public RelayCommand GoToSite_Village_Command { get; }

        public WildernessViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            GoToSite_Mine_Command = new RelayCommand(() =>
            {
                _controller.GoToNextApplicationState(new ExplorationPayload { Site = "Mine" });
            });

            GoToSite_Village_Command = new RelayCommand(() =>
            {
                _controller.GoToNextApplicationState(new ExplorationPayload { Site = "Village" });
            });
        }
    }
}
