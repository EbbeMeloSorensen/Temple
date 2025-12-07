using Craft.Math;

namespace Temple.Application.State;

public class ApplicationData
{
    public HashSet<string> BattlesWon { get; }
    public Vector2D? ExplorationPosition { get; set; }
    public double? ExplorationOrientation { get; set; }

    public ApplicationData()
    {
        BattlesWon = new HashSet<string>();
    }
}

