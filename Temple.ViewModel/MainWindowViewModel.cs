using Craft.ViewModels.Dialogs;
using GalaSoft.MvvmLight;
using MediatR;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Interfaces.Readers;
using Temple.Application.State;
using Temple.Infrastructure.GameConditions;
using Temple.ViewModel.DD;
using Temple.ViewModel.DD.Battle;
using Temple.ViewModel.DD.Dialogue;
using Temple.ViewModel.DD.Exploration;
using Temple.ViewModel.DD.InGameMenu;
using Temple.ViewModel.DD.ReadModels;
using Temple.ViewModel.DD.Wilderness;
using Temple.ViewModel.PR;
using Temple.ViewModel.Smurfs;

namespace Temple.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _applicationDialogService;
        private readonly ISiteRenderer _siteRenderer;
        private readonly IDialogueSessionFactory _dialogueSessionFactory;
        private readonly IFactsEstablishedReader _factsEstablishedReader;
        private readonly IKnowledgeGainedReader _knowledgeGainedReadModel;
        private readonly IQuestStatusReader _questStatusReadModel;
        private readonly ISitesUnlockedReader _sitesUnlockedReader;
        private readonly IBattlesWonReader _battlesWonReader;
        private readonly ApplicationController _controller;

        private string _currentApplicationStateAsText;
        private object _currentViewModel;

        public Action? ShutdownAction { get; set; }

        // This is just to show the current application state in the statusbar
        public string CurrentApplicationStateAsText
        {
            get => _currentApplicationStateAsText;
            private set => Set(ref _currentApplicationStateAsText, value);
        }

        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged();
            }
        }

        public MainWindowViewModel(
            IMediator mediator,
            IDialogService applicationDialogService,
            ISiteRenderer siteRenderer,
            IDialogueSessionFactory dialogueSessionFactory,
            ApplicationController controller)
        {
            _mediator = mediator;
            _applicationDialogService = applicationDialogService;
            _siteRenderer = siteRenderer;
            _dialogueSessionFactory = dialogueSessionFactory;
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _factsEstablishedReader = new FactsEstablishedReadModel(controller.EventBus);
            _knowledgeGainedReadModel = new KnowledgeGainedReadModel(controller.EventBus);
            _questStatusReadModel = new QuestStatusReadModel(controller.EventBus);
            _sitesUnlockedReader = new SitesUnlockedReadModel(controller.EventBus);
            _battlesWonReader = new BattlesWonReadModel(controller.EventBus);

            var dialogueQueryService = new GameQueryService(
                _factsEstablishedReader,
                _questStatusReadModel,
                _battlesWonReader,
                _sitesUnlockedReader);

            _dialogueSessionFactory.Initialize(
                _factsEstablishedReader,
                _knowledgeGainedReadModel,
                _questStatusReadModel,
                _sitesUnlockedReader,
                _battlesWonReader,
                dialogueQueryService,
                controller.EventBus);

            CurrentApplicationStateAsText = _controller.CurrentApplicationState.StateMachineState.ToString();

            _controller.ApplicationStateChanged += (applicationState) =>
            {
                CurrentApplicationStateAsText = applicationState.StateMachineState.ToString();

                switch (applicationState.StateMachineState)
                {
                    case StateMachineState.MainMenu:
                        CurrentViewModel = new HomeViewModel(_controller);
                        break;

                    case StateMachineState.ShuttingDown:
                        ShutdownAction?.Invoke();
                        break;

                    case StateMachineState.SmurfManagement:
                        CurrentViewModel = new MainWindowViewModel_Smurfs(_mediator, _controller);
                        break;

                    case StateMachineState.PeopleManagement:
                        CurrentViewModel = new MainWindowViewModel_PR(_mediator, _applicationDialogService, _controller);
                        break;

                    case StateMachineState.GameStartup:
                        _questStatusReadModel.Initialize(_controller.Quests);
                        _controller.ExitState();
                        break;

                    case StateMachineState.Interlude:
                        var interludeViewModel = new InterludeViewModel(_controller);
                        CurrentViewModel = interludeViewModel.Init(applicationState.Payload);
                        break;

                    case StateMachineState.Exploration:
                        var explorationViewModel = new ExplorationViewModel(_controller, _questStatusReadModel, _siteRenderer);
                        CurrentViewModel = explorationViewModel.Init(applicationState.Payload);
                        break;

                    case StateMachineState.Battle:
                        var battleViewModel = new BattleViewModel(_controller);
                        CurrentViewModel = battleViewModel.Init(applicationState.Payload);
                        break;

                    case StateMachineState.Dialogue:
                        var dialogueViewModel = new DialogueViewModel(
                            _controller,
                            _factsEstablishedReader,
                            _knowledgeGainedReadModel,
                            _questStatusReadModel,
                            _sitesUnlockedReader,
                            _battlesWonReader,
                            _dialogueSessionFactory);

                        CurrentViewModel = dialogueViewModel.Init(applicationState.Payload);
                        break;

                    case StateMachineState.InGameMenu:
                        var inGameMenuViewModel = new InGameMenuViewModel(_controller, _questStatusReadModel);
                        CurrentViewModel = inGameMenuViewModel.Init(applicationState.Payload);
                        break;

                    case StateMachineState.Wilderness:
                        CurrentViewModel = new WildernessViewModel(_controller);
                        break;

                    case StateMachineState.Defeat:
                        CurrentViewModel = new DefeatViewModel(_controller);
                        break;

                    case StateMachineState.Victory:
                        CurrentViewModel = new VictoryViewModel(_controller);
                        break;

                    default:
                    {
                        throw new InvalidOperationException("Unknown state");
                    }
                }
            };

            CurrentViewModel = new HomeViewModel(_controller);
        }

        public void ShutDownEngineIfRunning()
        {
            if (CurrentViewModel is ExplorationViewModel explorationViewModel)
            {
                explorationViewModel.Engine.HandleClosing();
            }
        }
    }
}
