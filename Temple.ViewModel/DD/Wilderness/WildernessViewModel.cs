using Craft.Math;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Common;

namespace Temple.ViewModel.DD.Wilderness
{
    public class WildernessViewModel : TempleViewModel
    {
        private readonly ApplicationController _controller;
        private readonly IGameQueryService _gameQueryService;

        public RelayCommand GoToInGameMenu_Command { get; }
        public RelayCommand GoToSite_Mine_Command { get; }
        public RelayCommand GoToSite_Village_Command { get; }
        public RelayCommand GoToSite_Graveyard_Command { get; }
        public RelayCommand GoToSite_Maze_Command { get; }

        public WildernessViewModel(
            ApplicationController controller,
            IGameQueryService gameQueryService)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _gameQueryService = gameQueryService ?? throw new ArgumentNullException(nameof(gameQueryService)); ;

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
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "mine" });
            });

            GoToSite_Village_Command = new RelayCommand(() =>
            {
                _controller.ApplicationData.ExplorationPosition = new Vector2D(14.5, -7.5);
                _controller.ApplicationData.ExplorationOrientation = 1.0 * Math.PI;
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "village" });
            });

            GoToSite_Graveyard_Command = new RelayCommand(() =>
            {
                _controller.ApplicationData.ExplorationPosition = new Vector2D(14.5, -7.5);
                _controller.ApplicationData.ExplorationOrientation = 1.0 * Math.PI;
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "graveyard" });
            });

            GoToSite_Maze_Command = new RelayCommand(() =>
            {
                _controller.ApplicationData.ExplorationPosition = new Vector2D(1.5, 0.5);
                _controller.ApplicationData.ExplorationOrientation = 0.5 * Math.PI;
                _controller.GoToNextApplicationState(new ExplorationPayload { SiteId = "maze" });
            });
        }
    }
}
