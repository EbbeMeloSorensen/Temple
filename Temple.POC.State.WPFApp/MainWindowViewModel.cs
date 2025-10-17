using System.Collections.ObjectModel;
using MediatR;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Smurfs;
using Temple.Application.Core;

namespace Temple.POC.State.WPFApp
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _buttonText;
        private readonly IMediator _mediator;
        private readonly ApplicationController _controller;
        private string _currentState;

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
            };

            StartWorkCommand = new RelayCommand(_controller.BeginWork);
            ShutdownCommand = new RelayCommand(_controller.Shutdown);
        }

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