using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.ViewModel.DD.Quests;

// Denne klasse overvåger ændringer i quest-tilstande. Den er bindeled mellem quest-logikken og brugergrænsefladen.
// Den publicerer et event, der bruges i forbindelse med opdatering af brugergrænsefladen
public sealed class QuestStatusReadModel : IQuestStatusReadModel
{
    private readonly Dictionary<string, QuestStatus> _quests =
        new Dictionary<string, QuestStatus>();

    public event EventHandler<QuestStatusChangedEventArgs>? QuestStatusChanged;

    public QuestStatusReadModel(
        QuestEventBus eventBus)
    {
        eventBus.Subscribe<QuestStateChangedEvent>(HandleQuestStateChanged);
        eventBus.Subscribe<QuestSatisfactionOfCompletionCriteriaChangedEvent>(HandleQuestSatisfactionOfCompletionCriteriaChanged);
    }

    // Det her er sådan list en diagnostisk ting, da cachen sædvanligvis er tom ved opstart
    public void Initialize(
        IReadOnlyCollection<Quest> quests)
    {
        quests.ToList().ForEach(quest =>
        {
            _quests[quest.Id] = new QuestStatus
            {
                QuestState = quest.State,
                AreCompletionCriteriaSatisfied = quest.AreCompletionCriteriaSatisfied
            };
        });
    }

    public QuestStatus GetQuestStatus(
        string questId)
    {
        return _quests.TryGetValue(questId, out QuestStatus status) ? status : new QuestStatus
        {
            QuestState = QuestState.Hidden, 
            AreCompletionCriteriaSatisfied = false
        } ;
    }

    public IEnumerable<string> Quests => _quests.Select(kvp =>
    {
        var questStatus = kvp.Value;
        var result = $"{kvp.Key}: {questStatus.QuestState}";

        if (questStatus.AreCompletionCriteriaSatisfied)
        {
            result = $"{result} (completion criteria satisfied)";
        }

        return result;
    });

    private void HandleQuestStateChanged(
        QuestStateChangedEvent e)
    {
        if (!_quests.TryGetValue(e.QuestId, out var status))
        {
            _quests.Add(e.QuestId, new QuestStatus
            {
                QuestState = e.NewState,
                AreCompletionCriteriaSatisfied = false
            });
        }
        else
        {
            _quests[e.QuestId].QuestState = e.NewState;
        }

        OnQuestStatusChanged(e.QuestId, _quests[e.QuestId]);
    }

    private void HandleQuestSatisfactionOfCompletionCriteriaChanged(
        QuestSatisfactionOfCompletionCriteriaChangedEvent e)
    {
        _quests[e.QuestId].AreCompletionCriteriaSatisfied = e.AreCompletionCriteriaSatisfied;

        OnQuestStatusChanged(e.QuestId, _quests[e.QuestId]);
    }

    private void OnQuestStatusChanged(
        string questId,
        QuestStatus questStatus)
    {
        QuestStatusChanged?.Invoke(this, new QuestStatusChangedEventArgs(questId, questStatus));
    }
}