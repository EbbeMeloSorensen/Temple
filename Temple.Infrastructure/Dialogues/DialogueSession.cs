using Craft.DataStructures.Graph;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests.Events;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSession : IDialogueSession
{
    private QuestEventBus _eventBus;
    private GraphAdjacencyList<DialogueVertex, LabelledEdge> _graph;
    private int _activeVertexId;

    public string NPCPortraitPath { get; }

    public string CurrentNPCText => ((DialogueVertex) _graph.GetVertex(_activeVertexId)).Text;

    public IReadOnlyList<DialogueChoice> AvailableChoices
    {
        get
        {
            return _graph.OutgoingEdges(_activeVertexId)
                .Select(_ => new DialogueChoice
                {
                    Id = _.VertexId2,
                    Text = ((LabelledEdge)_).Label
                })
                .ToList();
        }
    }

    public DialogueSession(
        QuestEventBus eventBus,
        string npcId,
        GraphAdjacencyList<DialogueVertex, LabelledEdge> graph)
    {
        _eventBus = eventBus;
        _graph = graph;

        NPCPortraitPath = npcId switch
        {
            "innkeeper" => "DD/Images/Innkeeper.png",
            "guard" => "DD/Images/Guard.jpg",
            "captain" => "DD/Images/Captain.png",
            _ => throw new InvalidOperationException("Unknown npcId")
        };

        _activeVertexId = 0;
    }

    public void SelectChoice(
        int choiceId)
    {
        _activeVertexId = choiceId;

        switch (((DialogueVertex)_graph.GetVertex(_activeVertexId)).GameEventTrigger)
        {
            case QuestDiscoveredEventTrigger questDiscoveredEventTrigger:
                _eventBus.Publish(new QuestDiscoveredEvent(questDiscoveredEventTrigger.QuestId));
                break;
            case QuestAcceptedEventTrigger questAcceptedEventTrigger:
                _eventBus.Publish(new QuestAcceptedEvent(questAcceptedEventTrigger.QuestId));
                break;
        }
    }

    public bool IsFinished => !AvailableChoices.Any();
}