using Craft.DataStructures.Graph;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Quests.Events;
using Temple.Infrastructure.Dialogues.GameEventTriggers;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSession : IDialogueSession
{
    private IKnowledgeGainedReader _knowledgeGainedReadModel;
    private QuestEventBus _eventBus;

    private GraphAdjacencyList<DialogueVertex, DialogueEdge> _graph;
    private int _activeVertexId;

    public string NPCPortraitPath { get; }

    public string CurrentNPCText => ((DialogueVertex) _graph.GetVertex(_activeVertexId)).Text;

    public IReadOnlyList<DialogueChoice> AvailableChoices
    {
        get
        {
            // Denne skal have adgang til en readmodel for, hvilken knowledge, der er
            return _graph.OutgoingEdges(_activeVertexId)
                .Where(_ => ((DialogueEdge)_).KnowledgeRequired == null ||
                            _knowledgeGainedReadModel.KnowledgeGained.Contains(((DialogueEdge)_).KnowledgeRequired))
                .Select(_ => new DialogueChoice
                {
                    Id = _.VertexId2,
                    Text = ((DialogueEdge)_).Text
                })
                .ToList();
        }
    }

    public DialogueSession(
        IKnowledgeGainedReader knowledgeGainedReadModel,
        QuestEventBus eventBus,
        string npcId,
        GraphAdjacencyList<DialogueVertex, DialogueEdge> graph)
    {
        _knowledgeGainedReadModel = knowledgeGainedReadModel;
        _eventBus = eventBus;
        _graph = graph;

        NPCPortraitPath = npcId switch
        {
            "innkeeper" => "DD/Images/Innkeeper.png",
            "alyth" => "DD/Images/Alyth.png",
            "ethon" => "DD/Images/Ethon.png",
            "guard" => "DD/Images/Guard.jpg",
            "captain" => "DD/Images/Captain.png",
            "lortimer" => "DD/Images/Guard.jpg",
            "nebbish" => "DD/Images/Guard.jpg",
            _ => throw new InvalidOperationException("Unknown npcId")
        };

        _activeVertexId = 0;
        PossiblySwitchQuestState();
    }

    public void SelectChoice(
        int choiceId)
    {
        _activeVertexId = choiceId;
        PossiblySwitchQuestState();
    }

    public bool IsFinished => !AvailableChoices.Any();

    private void PossiblySwitchQuestState()
    {
        switch (((DialogueVertex)_graph.GetVertex(_activeVertexId)).GameEventTrigger)
        {
            case FactEstablishedEventTrigger factEstablishedEventTrigger:
                _eventBus.Publish(new FactEstablishedEvent(factEstablishedEventTrigger.FactId));
                break;
            case KnowledgeGainedEventTrigger knowledgeGainedEventTrigger:
                _eventBus.Publish(new KnowledgeGainedEvent(knowledgeGainedEventTrigger.KnowledgeId));
                break;
            case QuestDiscoveredEventTrigger questDiscoveredEventTrigger:
                _eventBus.Publish(new QuestDiscoveredEvent(questDiscoveredEventTrigger.QuestId));
                break;
            case QuestAcceptedEventTrigger questAcceptedEventTrigger:
                _eventBus.Publish(new QuestAcceptedEvent(questAcceptedEventTrigger.QuestId));
                break;
            case SiteUnlockedEventTrigger siteUnlockedEventTrigger:
                _eventBus.Publish(new SiteUnlockedEvent(siteUnlockedEventTrigger.SiteId));
                break;
        }
    }
}