using Temple.Domain.Entities.DD.Battle;

namespace Temple.Application.Interfaces;

public interface IBattleSceneFactory
{
    public Scene SetupBattleScene(
        List<Creature> party,
        string battleSceneId,
        string? entranceId);
}