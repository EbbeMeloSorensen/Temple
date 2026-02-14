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
    private IBattlesWonReader _battlesWonReader;
    private IDialogueQueryService _dialogueQueryService;
    private QuestEventBus _eventBus;

    public void Initialize(
        IFactsEstablishedReader factsEstablishedReader,
        IKnowledgeGainedReader knowledgeGainedReader,
        IQuestStatusReader questStatusReader,
        ISitesUnlockedReader sitesUnlockedReader,
        IBattlesWonReader battlesWonReader,
        IDialogueQueryService dialogueQueryService,
        QuestEventBus eventBus)
    {
        _factsEstablishedReader = factsEstablishedReader;
        _knowledgeGainedReader = knowledgeGainedReader;
        _questStatusReader = questStatusReader;
        _sitesUnlockedReader = sitesUnlockedReader;
        _battlesWonReader = battlesWonReader;
        _dialogueQueryService = dialogueQueryService;
        _eventBus = eventBus;
    }

    public IDialogueSession GetDialogueSession(
        string npcId)
    {
        return new DialogueSession(
            _knowledgeGainedReader,
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
        if (graph.Condition == null)
        {
            return true;
        }

        return graph.Condition.Evaluate(_dialogueQueryService);

        switch (graph.Condition)
        {
            case QuestStatusCondition questStatusCondition:
                if (!_questStatusReader.GetQuestStatus(questStatusCondition.QuestId)
                        .Equals(questStatusCondition.RequiredStatus))
                {
                    return false;
                }
                break;
            case FactEstablishedCondition factEstablishedCondition:
            {
                if (!_factsEstablishedReader.FactEstablished(factEstablishedCondition.FactId))
                {
                    return false;
                }
                break;
            }
            case BattleWonCondition battleWonCondition:
            {
                if (!_battlesWonReader.BattleWon(battleWonCondition.BattleId))
                {
                    return false;
                }
                break;
            }
            default:
                throw new NotImplementedException();
        }

        return true;
    }
}