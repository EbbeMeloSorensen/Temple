using Temple.Application.Core;
using Temple.Application.Interfaces.Readers;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        IFactsEstablishedReader factsEstablishedReader,
        IKnowledgeGainedReader knowledgeGainedReader,
        IQuestStatusReader questStatusReader,
        ISitesUnlockedReader sitesUnlockedReader,
        QuestEventBus eventBus,
        string npcId);
}