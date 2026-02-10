using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        IKnowledgeGainedReader knowledgeGainedReadModel,
        IQuestStatusReader questStatusReadModel,
        QuestEventBus eventBus,
        string npcId);
}