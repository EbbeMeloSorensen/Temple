namespace Temple.Application.State;

public class ApplicationStatePayload
{
    public string? Site { get; init; }
    public string? EnemyGroup { get; init; }
    public string? NpcId { get; init; }

    public static ApplicationStatePayload ForExploration(string site) => new() { Site = site };
    public static ApplicationStatePayload ForBattle(string enemyGroup) => new() { EnemyGroup = enemyGroup };
    public static ApplicationStatePayload ForDialog(string npcId) => new() { NpcId = npcId };
}