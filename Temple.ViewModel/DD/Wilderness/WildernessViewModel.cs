using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.Wilderness
{
    public class WildernessViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;

        public RelayCommand GoToSite_Mine { get; }
        public RelayCommand GoToSite_Graveyard { get; }
        public RelayCommand GoToSite_Village { get; }

        public WildernessViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            GoToSite_Mine = new RelayCommand(() =>
            {
                _controller.GoToNextApplicationState(new ExplorationPayload { Site = "Mine" });
            });

            GoToSite_Graveyard = new RelayCommand(() =>
            {
                _controller.GoToNextApplicationState(new ExplorationPayload { Site = "Graveyard" });
            });

            GoToSite_Village = new RelayCommand(() =>
            {
                _controller.GoToNextApplicationState(new ExplorationPayload { Site = "Village" });
            });
        }
    }
}
