using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        IKnowledgeGainedReadModel knowledgeGainedReadModel,
        IQuestStatusReadModel questStatusReadModel,
        QuestEventBus eventBus,
        string npcId)
    {
        return new DialogueSession(
            knowledgeGainedReadModel,
            eventBus,
            npcId,
            GenerateGraph_Dialogue(questStatusReadModel, npcId));
    }

    private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Dialogue(
        IQuestStatusReadModel questStatusReadModel,
        string npcId)
    {
        var dialogueGraphs =
            DialogueIO.ReadDialogueGraphListFromFile($"DD//Assets//DialogueGraphCollections//{npcId}.json");

        // Filtrer de grafer fra, som ikke kvalificerer
        dialogueGraphs = dialogueGraphs.Where(graph => DialogueGraphMeetsConditions(graph, questStatusReadModel));

        // Returner den af de kvalificerende grafer, som har den højeste prioritet
        var result = dialogueGraphs
            .OrderByDescending(_ => _.Priority)
            .First().Graph;

        result.WriteToFile(@"C:\Temp\CurrentDialogueGraph.dot", Format.Dot);

        return result;
    }

    private bool DialogueGraphMeetsConditions(
        DialogueGraph graph,
        IQuestStatusReadModel questStatusReadModel)
    {
        if (graph.Conditions == null || !graph.Conditions.Any())
        {
            return true;
        }

        foreach (var condition in graph.Conditions)
        {
            switch (condition)
            {
                case QuestStatusCondition questStatusCondition:
                    if (!questStatusReadModel.GetQuestStatus(questStatusCondition.QuestId)
                            .Equals(questStatusCondition.RequiredStatus))
                    {
                        return false;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        return true;
    }
}