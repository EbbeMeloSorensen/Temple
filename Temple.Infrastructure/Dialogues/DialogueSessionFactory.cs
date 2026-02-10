using Craft.DataStructures.Graph;
using Craft.DataStructures.IO;
using Temple.Application.Core;
using Temple.Application.Interfaces;
using Temple.Application.Interfaces.Readers;
using Temple.Infrastructure.Dialogues.DialogueGraphConditions;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.Dialogues;

public class DialogueSessionFactory : IDialogueSessionFactory
{
    private IFactsEstablishedReader _factsEstablishedReader;
    private IKnowledgeGainedReader _knowledgeGainedReader;
    private IQuestStatusReader _questStatusReader;
    private ISitesUnlockedReader _sitesUnlockedReader;
    private QuestEventBus _eventBus;

    public void Initialize(
        IFactsEstablishedReader factsEstablishedReader,
        IKnowledgeGainedReader knowledgeGainedReader,
        IQuestStatusReader questStatusReader,
        ISitesUnlockedReader sitesUnlockedReader,
        QuestEventBus eventBus)
    {
        _factsEstablishedReader = factsEstablishedReader;
        _knowledgeGainedReader = knowledgeGainedReader;
        _questStatusReader = questStatusReader;
        _sitesUnlockedReader = sitesUnlockedReader;
        _eventBus = eventBus;
    }

    public IDialogueSession GetDialogueSession(
        string npcId)
    {
        return new DialogueSession(
            _knowledgeGainedReader,
            _eventBus,
            npcId,
            GenerateGraph_Dialogue(_factsEstablishedReader, _questStatusReader, npcId));
    }

    private GraphAdjacencyList<DialogueVertex, DialogueEdge> GenerateGraph_Dialogue(
        IFactsEstablishedReader factsEstablishedReader,
        IQuestStatusReader questStatusReader,
        string npcId)
    {
        var dialogueGraphs =
            DialogueIO.ReadDialogueGraphListFromFile($"DD//Assets//DialogueGraphCollections//{npcId}.json");

        // Filtrer de grafer fra, som ikke kvalificerer
        dialogueGraphs = dialogueGraphs.Where(graph => DialogueGraphMeetsConditions(
            graph, factsEstablishedReader, questStatusReader));

        // Returner den af de kvalificerende grafer, som har den højeste prioritet
        var result = dialogueGraphs
            .OrderByDescending(_ => _.Priority)
            .First().Graph;

        result.WriteToFile(@"C:\Temp\CurrentDialogueGraph.dot", Format.Dot);

        return result;
    }

    private bool DialogueGraphMeetsConditions(
        DialogueGraph graph,
        IFactsEstablishedReader factsEstablishedReader,
        IQuestStatusReader questStatusReader)
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
                    if (!questStatusReader.GetQuestStatus(questStatusCondition.QuestId)
                            .Equals(questStatusCondition.RequiredStatus))
                    {
                        return false;
                    }
                    break;
                case FactEstablishedCondition factEstablishedCondition:
                {
                    if (!factsEstablishedReader.FactEstablished(factEstablishedCondition.FactId))
                    {
                        return false;
                    }
                }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        return true;
    }
}