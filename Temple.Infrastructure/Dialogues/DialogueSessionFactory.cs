using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;
using Temple.Application.Core;
using Temple.Application.DD;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Quests;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    private IQuestStatusReadModel _questStatusReadModel;

    public IDialogueSession GetDialogueSession(
        IQuestStatusReadModel questStatusReadModel,
        QuestEventBus eventBus,
        string npcId)
    {
        _questStatusReadModel = questStatusReadModel;

        return new DialogueSession(eventBus, npcId, GenerateGraph_Dialogue(npcId));
    }

    private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Dialogue(
        string npcId)
    {
        var dialogueGraphs =
            DialogueIO.ReadDialogueGraphListFromFile($"DD//Assets//DialogueGraphCollections//{npcId}.json");

        // Filtrer de grafer fra, som ikke kvalificerer
        dialogueGraphs = dialogueGraphs.Where(DialogueGraphMeetsConditions);

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
        if (graph.Conditions == null || !graph.Conditions.Any())
        {
            return true;
        }

        foreach (var condition in graph.Conditions)
        {
            var status = _questStatusReadModel.GetQuestStatus(condition.QuestId);

            if (!status.Equals(condition.RequiredStatus))
            {
                return false;
            }
        }

        return true;
    }
}