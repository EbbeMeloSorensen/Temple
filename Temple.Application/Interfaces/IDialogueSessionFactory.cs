using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    public IDialogueSession GetDialogueSession(
        IKnowledgeGainedReadModel knowledgeGainedReadModel,
        IQuestStatusReadModel questStatusReadModel,
        QuestEventBus eventBus,
        string npcId);
}