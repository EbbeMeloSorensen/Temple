using GalaSoft.MvvmLight.Command;
using Temple.Application.Core;
using Temple.Application.State.Payloads;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.ViewModel.DD.Quests;

namespace Temple.ViewModel.DD.Dialogue;

public class DialogueViewModel : TempleViewModel
{
    private readonly ApplicationController _controller;
    private readonly QuestStateReadModel _questStateReadModel;
    private string _title;
    private string _npcPortraitPath;
    private string _message;
    private string? _questId;
    private bool _takeQuestPossible;

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

    // Find lige på noget bedre - det er betegnelsen for noget, der bliver sagt af npc'en
    public string Message
    {
        get { return _message; }
        set
        {
            _message = value;
            RaisePropertyChanged();
        }
    }

    public bool TakeQuestPossible
    {
        get => _takeQuestPossible;
        private set
        {
            _takeQuestPossible = value;
            RaisePropertyChanged();
        }
    }

    public RelayCommand Leave_Command { get; }
    public RelayCommand TakeQuest_Command { get; }

    public DialogueViewModel(
        ApplicationController controller,
        QuestStateReadModel questStateReadModel)
    {
        _controller = controller ?? throw new ArgumentNullException(nameof(controller));
        _questStateReadModel = questStateReadModel ?? throw new ArgumentNullException(nameof(questStateReadModel));

        _questStateReadModel.QuestStateChanged += HandleQuestStateChanged;

        Leave_Command = new RelayCommand(() =>
        {
            _controller.GoToNextApplicationState(new ExplorationPayload
            {
                SiteId = _controller.ApplicationData.CurrentSiteId
            });
        });

        TakeQuest_Command = new RelayCommand(() =>
        {
            Message = "Awesome. Good luck!";
            _controller.EventBus.Publish(new QuestAcceptedEvent("rat_infestation"));
            TakeQuestPossible = false;
        });
    }

    private void HandleQuestStateChanged(
        object? sender,
        QuestStateChangedEventArgs e)
    {
        if (_questId == null || e.QuestId != _questId) return;

        switch (e.QuestState)
        {
            case QuestState.Available:
                Message = "Greetings adventurer! I have a quest for you. Will you take it?";
                TakeQuestPossible = true;
                break;
            case QuestState.Completed:
                Message = "Welcome back. I see you completed my quest. Here is your reward";
                break;
        }
    }

    public override TempleViewModel Init(
        ApplicationStatePayload payload)
    {
        var dialoguePayload = payload as DialoguePayload
                                 ?? throw new ArgumentException("Payload is not of type DialoguePayload", nameof(payload));

        var dialogueData = DialogueDataFactory.GenerateDialogueData(dialoguePayload.NPCId);

        Title = dialoguePayload.NPCId;
        NPCPortraitPath = dialogueData.NPCPortraitPath;

        if (dialogueData.QuestId != null)
        {
            _questId = dialogueData.QuestId;

            var questState = _questStateReadModel.GetQuestState(_questId);

            // HVIS status er en af disse 3, så opdaterer vi beskeden
            Message = questState switch
            {
                QuestState.Available => "Hello again, do you want the quest, after all?",
                QuestState.Active => "Good luck in handling the quest!",
                QuestState.Completed => "Thank you for completing my quest!",
                _ => Message
            };

            if (questState == QuestState.Available)
            {
                TakeQuestPossible = true;
            }

            // Her poster vi et event til bussen om at der er en dialog i gang med en given pc.
            // Det trigger så, at en givne quest gøres tilgængelig ELLER (hvis objectives er klaret) at den markeres som fuldført
            _controller.EventBus.Publish(new DialogueEvent(dialoguePayload.NPCId));
        }
        else
        {
            Message = "Leave me alone";
        }

        return this;
    }

    public override void Cleanup()
    {
        // Unsubscribe to prevent memory leaks
        _questStateReadModel.QuestStateChanged -= HandleQuestStateChanged;
        base.Cleanup();
    }
}
