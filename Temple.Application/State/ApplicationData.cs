using Craft.Math;
using Temple.Domain.Entities.DD;

namespace Temple.Application.State;

public class ApplicationData
{
    public List<Creature> Party { get; set; }
    public HashSet<string> BattlesWon { get; }
    public Vector2D? ExplorationPosition { get; set; }
    public double? ExplorationOrientation { get; set; }

    public ApplicationData()
    {
        Party = new List<Creature>();
        BattlesWon = new HashSet<string>();
    }
}

