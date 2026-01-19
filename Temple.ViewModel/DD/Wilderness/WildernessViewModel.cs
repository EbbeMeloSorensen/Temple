using Craft.Math;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.Wilderness
{
    public class WildernessViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;

        public RelayCommand GoToInGameMenu_Command { get; }
        public RelayCommand GoToSite_Mine_Command { get; }
        public RelayCommand GoToSite_Village_Command { get; }
        public RelayCommand GoToSite_Graveyard_Command { get; }

        public WildernessViewModel(
            ApplicationController controller)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            GoToInGameMenu_Command = new RelayCommand(() =>
            {
                var payload = new InGameMenuPayload
                {
                    PayloadForNextState = new WildernessPayload()
                };

                _controller.GoToNextApplicationState(payload);
            });

            GoToSite_Mine_Command = new RelayCommand(() =>
            {
                _controller.ApplicationData.ExplorationPosition = new Vector2D(0.5, -0.5);
                _controller.ApplicationData.ExplorationOrientation = 0.5 * Math.PI;
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "Mine" });
            });

            GoToSite_Village_Command = new RelayCommand(() =>
            {
                _controller.ApplicationData.ExplorationPosition = new Vector2D(14.5, -7.5);
                _controller.ApplicationData.ExplorationOrientation = 1.0 * Math.PI;
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "Village" });
            });

            GoToSite_Graveyard_Command = new RelayCommand(() =>
            {
                _controller.ApplicationData.ExplorationPosition = new Vector2D(14.5, -7.5);
                _controller.ApplicationData.ExplorationOrientation = 1.0 * Math.PI;
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "Graveyard" });
            });
        }
    }
}
