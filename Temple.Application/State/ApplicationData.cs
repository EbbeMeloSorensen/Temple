using Craft.Math;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.Application.State;

public class ApplicationData
{
    public List<Creature> Party { get; private set; } = new();
    public HashSet<string> KnowledgeGained { get; private set; } = new();
    public HashSet<string> BattlesWon { get; private set; } = new();
    public string CurrentSiteId { get; set; }
    public Vector2D? ExplorationPosition { get; set; }
    public double? ExplorationOrientation { get; set; }
}

