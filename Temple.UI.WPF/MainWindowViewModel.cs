using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MediatR;
using System.Collections.ObjectModel;
using Temple.Application.Core;
using Temple.Application.Smurfs;
using Temple.ViewModel;

namespace Temple.UI.WPF
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _buttonText;
        private readonly IMediator _mediator;
        private readonly ApplicationController _controller;
        private string _currentState;
        private object _currentViewModel;

        public string ButtonText
        {
            get => _buttonText;
            set
            {
                _buttonText = value;
                RaisePropertyChanged();
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


        public ObservableCollection<SmurfDto> Smurfs { get; } = new();

        public RelayCommand LoadSmurfsCommand { get; }
        public RelayCommand StartWorkCommand { get; }
        public RelayCommand ShutdownCommand { get; }

        public MainWindowViewModel(
            IMediator mediator,
            ApplicationController controller)
        {
            _mediator = mediator;
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            _buttonText = "Populate list with smurfs";

            LoadSmurfsCommand = new RelayCommand(async () => await LoadSmurfsAsync());

            CurrentState = _controller.CurrentState.ToString();

            _controller.StateChanged += (_, e) =>
            {
                CurrentState = e.NewState.ToString();

                if (CurrentState == "Idle")
                {
                    ShowHome();
                }
                else
                {
                    ShowSettings();
                }
            };

            StartWorkCommand = new RelayCommand(_controller.BeginWork);
            ShutdownCommand = new RelayCommand(_controller.Shutdown);

            ShowHome();
        }

        public void ShowHome() => CurrentViewModel = new HomeViewModel();
        public void ShowSettings() => CurrentViewModel = new SettingsViewModel();

        private async Task LoadSmurfsAsync()
        {
            var smurfs = await _mediator.Send(new List.Query { Params = new SmurfParams() });

            Smurfs.Clear();
            foreach (var smurfDto in smurfs.Value)
            {
                Smurfs.Add(smurfDto);
            }
        }
    }
}