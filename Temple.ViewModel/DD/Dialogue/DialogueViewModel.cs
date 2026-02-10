using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.State.Payloads;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private readonly IKnowledgeGainedReadModel _knowledgeGainedReadModel;
    private readonly IQuestStatusReadModel _questStatusReadModel;
    private readonly IDialogueSessionFactory _dialogueSessionFactory;
    private IDialogueSession _dialogueSession;
    private string _title;
    private string _npcPortraitPath;
    private string _npcText;

    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            RaisePropertyChanged();
        }
    }

    public string NPCPortraitPath
    {
        get { return _npcPortraitPath; }
        set
        {
            _npcPortraitPath = value;
            RaisePropertyChanged();
        }
    }

    public string NPCText
    {
        get { return _npcText; }
        set
        {
            _npcText = value;
            RaisePropertyChanged();
        }
    }

    public ObservableCollection<DialogueOptionViewModel> Options { get; }

    public RelayCommand<int> SelectOption_Command { get; }

    public DialogueViewModel(
        ApplicationController controller,
        IKnowledgeGainedReadModel knowledgeGainedReadModel,
        IQuestStatusReadModel questStatusReadModel,
        IDialogueSessionFactory dialogueSessionFactory)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _knowledgeGainedReadModel = knowledgeGainedReadModel ?? throw new ArgumentNullException(nameof(knowledgeGainedReadModel));
        _questStatusReadModel = questStatusReadModel ?? throw new ArgumentNullException(nameof(questStatusReadModel));
        _dialogueSessionFactory = dialogueSessionFactory ?? throw new ArgumentNullException(nameof(dialogueSessionFactory));

        SelectOption_Command = new RelayCommand<int>(optionId =>
        {
            _dialogueSession.SelectChoice(optionId);

            if (_dialogueSession.IsFinished)
            {
                _controller.GoToNextApplicationState(new ExplorationPayload
                {
                    SiteId = _controller.ApplicationData.CurrentSiteId
                });
            }
            else
            {
                Update();
            }
        });

        Options = new ObservableCollection<DialogueOptionViewModel>();
    }

    private void Update()
    {
        Options.Clear();
        NPCText = _dialogueSession.CurrentNPCText;

        _dialogueSession.AvailableChoices.ToList().ForEach(option =>
        {
            Options.Add(new DialogueOptionViewModel(option.Id, option.Text));
        });
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var dialoguePayload = payload as DialoguePayload
                                 ?? throw new ArgumentException("Payload is not of type DialoguePayload", nameof(payload));

        _dialogueSession = _dialogueSessionFactory.GetDialogueSession(
            _knowledgeGainedReadModel,
            _questStatusReadModel,
            _controller.EventBus,
            dialoguePayload.NPCId);

        Title = dialoguePayload.NPCId;
        NPCPortraitPath = _dialogueSession.NPCPortraitPath;

        _controller.EventBus.Publish(new DialogueEvent(dialoguePayload.NPCId));

        Update();

        return this;
    }
}
