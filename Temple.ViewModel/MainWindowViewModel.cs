using System.Collections.ObjectModel;
using MediatR;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;
using Temple.Application.Core;
using Temple.ViewModel.PR;
using Temple.ViewModel.Smurfs;

namespace Temple.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _applicationDialogService;
        private readonly ApplicationController _controller;
        private string _currentState;
        private MainWindowViewModel_Smurfs _mainWindowViewModel_Smurfs;
        private MainWindowViewModel_PR _mainWindowViewModel_PR;
        private object _currentViewModel;

        public Action? ShutdownAction { get; set; }

        public HomeViewModel HomeViewModel { get; }

        public MainWindowViewModel_Smurfs MainWindowViewModel_Smurfs
        {
            get
            {
                return _mainWindowViewModel_Smurfs ??= new MainWindowViewModel_Smurfs(_mediator, _controller);
            }
        }

        public MainWindowViewModel_PR MainWindowViewModel_PR
        {
            get
            {
                return _mainWindowViewModel_PR ??= new MainWindowViewModel_PR(_mediator, _applicationDialogService, _controller);
            }
        }

        public string CurrentState
        {
            get => _currentState;
            private set => Set(ref _currentState, value);
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
            ApplicationController controller)
        {
            _mediator = mediator;
            _applicationDialogService = applicationDialogService;
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            HomeViewModel = new HomeViewModel(_controller);

            CurrentState = _controller.CurrentState.ToString();

            _controller.StateChanged += (_, e) =>
            {
                CurrentState = e.NewState.ToString();

                switch (CurrentState)
                {
                    case "Idle":
                        CurrentViewModel = HomeViewModel;
                        break;
                    case "ShuttingDown":
                        ShutdownAction?.Invoke();
                        break;
                    case "SmurfManagement":
                        CurrentViewModel = MainWindowViewModel_Smurfs;
                        break;
                    case "PeopleManagement":
                        CurrentViewModel = MainWindowViewModel_PR;
                        break;
                    default:
                        throw new InvalidOperationException("Invalid operation");
                }
            };

            CurrentViewModel = new HomeViewModel(_controller);
        }
    }
}
