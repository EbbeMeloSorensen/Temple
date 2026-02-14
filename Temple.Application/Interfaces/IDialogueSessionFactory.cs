using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    void Initialize(
        IFactsEstablishedReader factsEstablishedReader,
        IKnowledgeGainedReader knowledgeGainedReader,
        IQuestStatusReader questStatusReader,
        ISitesUnlockedReader sitesUnlockedReader,
        IBattlesWonReader battlesWonReader,
        IDialogueQueryService dialogueQueryService,
        QuestEventBus eventBus);

    public IDialogueSession GetDialogueSession(
        string npcId);
}