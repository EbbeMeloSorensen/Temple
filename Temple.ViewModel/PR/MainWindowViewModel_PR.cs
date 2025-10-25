using System.Collections.ObjectModel;
using System.Windows;
using MediatR;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Craft.ViewModels.Dialogs;
using Temple.Application.Core;

namespace Temple.ViewModel.PR
{
    public class MainWindowViewModel_PR : ViewModelBase
    {
        private readonly IMediator _mediator;
        private readonly IDialogService _dialogService;
        private readonly ApplicationController _controller;

        public ObservableCollection<string> Items { get; } = new();

        public RelayCommand FindPeopleCommand { get; }
        public RelayCommand ExitCommand{ get; }
        public RelayCommand<object> CreatePersonCommand { get; }

        public MainWindowViewModel_PR(
            IMediator mediator,
            IDialogService dialogService,
            ApplicationController controller)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            FindPeopleCommand = new RelayCommand(async () => await FindPeopleAsync());
            CreatePersonCommand = new RelayCommand<object>(CreatePerson, CanCreatePerson);
            ExitCommand = new RelayCommand(_controller.GoToHome);
        }

        private async Task FindPeopleAsync()
        {
            var command = new Application.People.List.Query
            {
                Params = new Application.People.PersonParams()
            };

            var personDtos = await _mediator.Send(command);

            Items.Clear();
            foreach (var personDto in personDtos.Value)
            {
                Items.Add(personDto.FirstName);
            }
        }

        private void CreatePerson(
            object owner)
        {
            var dialogViewModel = new CreateOrUpdatePersonDialogViewModel(_mediator);

            if (_dialogService.ShowDialog(dialogViewModel, owner as Window) != DialogResult.OK)
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
