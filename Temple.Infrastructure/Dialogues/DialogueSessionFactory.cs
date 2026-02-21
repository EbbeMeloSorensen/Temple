using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;
using Temple.Domain.Entities.DD.Common;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    private IGameQueryService _gameQueryService;
    private QuestEventBus _eventBus;

    public void Initialize(
        IGameQueryService gameQueryService,
        QuestEventBus eventBus)
    {
        _gameQueryService = gameQueryService;
        _eventBus = eventBus;
    }

    public IDialogueSession GetDialogueSession(
        string npcId)
    {
        return new DialogueSession(
            _gameQueryService,
            _eventBus,
            npcId,
            GenerateGraph_Dialogue(npcId));
    }

    private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Dialogue(
        string npcId)
    {
        var dialogueGraphs =
            DialogueIO.ReadDialogueGraphListFromFile($"DD//Assets//DialogueGraphCollections//{npcId}.json");

        // Filtrer de grafer fra, som ikke kvalificerer
        dialogueGraphs = dialogueGraphs.Where(graph => DialogueGraphMeetsConditions(graph));

        // Returner den af de kvalificerende grafer, som har den højeste prioritet
        var result = dialogueGraphs
            .OrderByDescending(_ => _.Priority)
            .First().Graph;

        result.WriteToFile(@"C:\Temp\CurrentDialogueGraph.dot", Format.Dot);

        return result;
    }

    private bool DialogueGraphMeetsConditions(
        DialogueGraph graph)
    {
        return graph.Condition == null || graph.Condition.Evaluate(_gameQueryService);
    }
}