using MediatR;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Temple.Application.Smurfs;

namespace Temple.POC.AvaloniaApp.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _greeting = "Hello from Avalonia!";
        private readonly IMediator _mediator;

        public string Greeting
        {
            get => _greeting;
            set
            {
                if (_greeting != value)
                {
                    _greeting = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> Items { get; } = new();

        public ICommand UpdateGreetingCommand { get; }
        public ICommand LoadItemsCommand { get; }

        public MainWindowViewModel(IMediator mediator)
        {
            _mediator = mediator;
            UpdateGreetingCommand = new RelayCommand(UpdateGreeting);
            LoadItemsCommand = new RelayCommand(async () => await LoadItemsAsync());
        }

        private void UpdateGreeting()
        {
            Greeting = "You clicked the button!";
        }

        private async Task LoadItemsAsync()
        {
            Items.Clear();

            var result = await _mediator.Send(new List.Query
            {
                Params = new SmurfParams()
            });

            foreach (var item in result.Value)
                Items.Add(item.Name);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object? parameter) => _execute();
        public event EventHandler? CanExecuteChanged;
    }
}
