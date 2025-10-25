using System.Collections.ObjectModel;
using MediatR;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel.Smurfs
{
    public class MainWindowViewModel_Smurfs : ViewModelBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationController _controller;

        public ObservableCollection<string> Items { get; } = new();

        public RelayCommand FindSmurfsCommand { get; }
        public RelayCommand ExitCommand { get; }

        public MainWindowViewModel_Smurfs(
            IMediator mediator,
            ApplicationController controller)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));

            FindSmurfsCommand = new RelayCommand(async () => await FindSmurfsAsync());
            ExitCommand = new RelayCommand(_controller.GoToHome);
        }

        private async Task FindSmurfsAsync()
        {
            var command = new Application.Smurfs.List.Query
            {
                Params = new Application.Smurfs.SmurfParams()
            };

            var smurfDtos = await _mediator.Send(command);

            Items.Clear();
            foreach (var smurfDto in smurfDtos.Value)
            {
                Items.Add(smurfDto.Name);
            }
        }
    }
}
