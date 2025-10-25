using System.Collections.ObjectModel;
using System.Windows;
using MediatR;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using Temple.Application.Core;
using Temple.ViewModel.PR;

namespace Temple.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _applicationDialogService;
        private readonly ApplicationController _controller;
        private string _currentState;
        private object _currentViewModel;
        private AsyncCommand<object> _createPersonCommand;

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

        public ObservableCollection<string> Items { get; } = new();

        public RelayCommand LoadSmurfsCommand { get; }
        public RelayCommand LoadPeopleCommand { get; }
        public RelayCommand StartWorkCommand { get; }
        public RelayCommand ShutdownCommand { get; }

        public AsyncCommand<object> CreatePersonCommand
        {
            get { return _createPersonCommand ?? (_createPersonCommand = new AsyncCommand<object>(CreatePerson, CanCreatePerson)); }
        }

        public MainWindowViewModel(
            IMediator mediator,
            IDialogService applicationDialogService,
            ApplicationController controller)
        {
            _mediator = mediator;
            _applicationDialogService = applicationDialogService;
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            LoadSmurfsCommand = new RelayCommand(async () => await LoadSmurfsAsync());
            LoadPeopleCommand = new RelayCommand(async () => await LoadPeopleAsync());

            CurrentState = _controller.CurrentState.ToString();

            _controller.StateChanged += (_, e) =>
            {
                CurrentState = e.NewState.ToString();

                // Det skal nok ikke være nye hele tiden her...
                switch (CurrentState)
                {
                    case "Idle":
                        CurrentViewModel = new HomeViewModel(_controller);
                        break;
                    case "Working":
                        CurrentViewModel = new SettingsViewModel();
                        break;
                    case "PeopleManagement":
                        CurrentViewModel = new MainWindowViewModel_PR(_controller);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid operation");
                }
            };

            StartWorkCommand = new RelayCommand(_controller.BeginWork);
            ShutdownCommand = new RelayCommand(_controller.Shutdown);

            CurrentViewModel = new HomeViewModel(_controller);
        }

        private async Task LoadSmurfsAsync()
        {
            var smurfDtos = await _mediator.Send(new Application.Smurfs.List.Query { Params = new Application.Smurfs.SmurfParams() });

            Items.Clear();
            foreach (var smurfDto in smurfDtos.Value)
            {
                Items.Add(smurfDto.Name);
            }
        }

        private async Task LoadPeopleAsync()
        {
            var personDtos = await _mediator.Send(new Application.People.List.Query { Params = new Application.People.PersonParams() });

            Items.Clear();
            foreach (var personDto in personDtos.Value)
            {
                Items.Add(personDto.FirstName);
            }
        }

        private async Task CreatePerson(
            object owner)
        {
            var dialogViewModel = new CreateOrUpdatePersonDialogViewModel(_mediator);

            if (_applicationDialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
            {
                return;
            }

            //if (dialogViewModel.Person.End > DateTime.UtcNow)
            //{
            //    PersonListViewModel.AddPerson(dialogViewModel.Person);
            //}
        }

        private bool CanCreatePerson(
            object owner)
        {
            return true;
        }
    }
}
