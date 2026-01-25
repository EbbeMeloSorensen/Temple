using GalaSoft.MvvmLight;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueOptionViewModel : ViewModelBase
{
    private int _dialogueOptionId;
    private string _text;

    public DialogueOptionViewModel(
        int dialogueOptionId,
        string text)
    {
        _dialogueOptionId = dialogueOptionId;
        _text = text;
    }

    public string Text
    {
        get => _text;
        set
        {
            if (value == _text) return;

            _text = value;
            RaisePropertyChanged();
        }
    }
}