namespace Temple.Application.State;

public class ScenePayload
{
    public string? Site { get; init; }
    public string? EnemyGroup { get; init; }
    public string? NpcId { get; init; }

    public static ScenePayload ForExploration(string site) => new() { Site = site };
    public static ScenePayload ForBattle(string enemyGroup) => new() { EnemyGroup = enemyGroup };
    public static ScenePayload ForDialog(string npcId) => new() { NpcId = npcId };
}