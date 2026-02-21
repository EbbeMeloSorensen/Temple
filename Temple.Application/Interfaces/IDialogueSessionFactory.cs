using Temple.Domain.Entities.DD.Common;
using Temple.Application.Core;

namespace Temple.Application.Interfaces;

public interface IDialogueSessionFactory
{
    void Initialize(
        IGameQueryService gameQueryService,
        QuestEventBus eventBus);

    public IDialogueSession GetDialogueSession(
        string npcId);
}