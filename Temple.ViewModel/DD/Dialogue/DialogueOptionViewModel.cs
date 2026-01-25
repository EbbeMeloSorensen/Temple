using GalaSoft.MvvmLight;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueOptionViewModel : ViewModelBase
{
    private int _optionId;
    private string _text;

    public DialogueOptionViewModel(
        int optionId,
        string text)
    {
        _optionId = optionId;
        _text = text;
    }

    public int OptionId
    {
        get => _optionId;
        set
        {
            if (value == _optionId) return;

            _optionId = value;
            RaisePropertyChanged();
        }
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