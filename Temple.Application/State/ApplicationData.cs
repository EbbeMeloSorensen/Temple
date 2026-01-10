using Craft.Math;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Battle;

namespace Temple.Application.State;

public class ApplicationData
{
    public List<Creature> Party { get; private set; }
    public HashSet<string> BattlesWon { get; private set; }
    public string CurrentSite { get; set; }
    public Vector2D? ExplorationPosition { get; set; }
    public double? ExplorationOrientation { get; set; }

    public ApplicationData()
    {
        Party = new List<Creature>();
        BattlesWon = new HashSet<string>();
    }
}

