using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;

namespace Temple.ViewModel;

public sealed class GameSaveViewModel : ViewModelBase
{
    private readonly ApplicationController _controller;
    private readonly bool _canSave;
    private bool _isBusy;
    private string _statusMessage = "";

    public IReadOnlyList<int> Slots { get; } = new[] { 1, 2, 3 };
    public int SelectedSlot { get; set; } = 1;
    public bool IsInteractionEnabled => !_isBusy;
    public string StatusMessage
    {
        get => _statusMessage;
        private set => Set(ref _statusMessage, value);
    }

    public RelayCommand SaveCommand { get; }
    public RelayCommand LoadCommand { get; }

    public GameSaveViewModel(ApplicationController controller, bool canSave)
    {
        _controller = controller;
        _canSave = canSave;
        SaveCommand = new RelayCommand(async () => await SaveAsync(), () => _canSave && !_isBusy);
        LoadCommand = new RelayCommand(async () => await LoadAsync(), () => !_isBusy);
    }

    public Task SaveAsync() => RunAsync(save: true);
    public Task LoadAsync() => RunAsync(save: false);

    private async Task RunAsync(bool save)
    {
        if (_isBusy || (save && !_canSave)) return;
        _isBusy = true;
        RaisePropertyChanged(nameof(IsInteractionEnabled));
        SaveCommand.RaiseCanExecuteChanged();
        LoadCommand.RaiseCanExecuteChanged();
        var slot = SelectedSlot;
        StatusMessage = save ? "Saving…" : "Loading…";
        try
        {
            var result = save
                ? await _controller.SaveGameAsync(slot)
                : await _controller.LoadGameAsync(slot);
            StatusMessage = result.IsSuccess
                ? (save ? $"Saved to slot {slot}." : $"Loaded slot {slot}.")
                : result.Error;
        }
        finally
        {
            _isBusy = false;
            RaisePropertyChanged(nameof(IsInteractionEnabled));
            SaveCommand.RaiseCanExecuteChanged();
            LoadCommand.RaiseCanExecuteChanged();
        }
    }
}
