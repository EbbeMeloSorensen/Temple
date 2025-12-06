using GalaSoft.MvvmLight;
using Temple.Application.State;
using Temple.ViewModel.DD;

namespace Temple.ViewModel
{
    public class NavigationService
    {
        private readonly Func<HomeViewModel> _homeFactory;
        private readonly Func<InterludeViewModel> _interludeFactory;
        private readonly Func<BattleViewModel> _battleFactory;
        private readonly Func<ExplorationViewModel> _exploreAreaFactory;

        public NavigationService(
            Func<HomeViewModel> homeFactory,
            Func<InterludeViewModel> interludeFactory,
            Func<BattleViewModel> battleFactory,
            Func<ExplorationViewModel> exploreAreaFactory)
        {
            _homeFactory = homeFactory ?? throw new ArgumentNullException(nameof(homeFactory));
            _interludeFactory = interludeFactory ?? throw new ArgumentNullException(nameof(interludeFactory));
            _battleFactory = battleFactory ?? throw new ArgumentNullException(nameof(battleFactory));
            _exploreAreaFactory = exploreAreaFactory ?? throw new ArgumentNullException(nameof(exploreAreaFactory));
        }

        public ViewModelBase CreateViewModel(ApplicationState scene)
        {
            return scene.StateMachineState switch
            {
                StateMachineState.MainMenu => _homeFactory(),
                //SceneType.Interlude => _interludeFactory().Init(scene.Payload),
                //SceneType.Battle => _battleFactory().Init(scene.Payload),
                //SceneType.Exploration => _exploreAreaFactory().Init(scene.Payload),
                _ => throw new NotImplementedException()
            };
        }
    }
}
