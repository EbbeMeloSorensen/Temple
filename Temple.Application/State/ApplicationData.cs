using Craft.Math;
using Temple.Domain.Entities.DD;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        ExplorationPosition = new Vector2D(0.5, -0.5);
        ExplorationOrientation = 0.5 * Math.PI;
    }
}

