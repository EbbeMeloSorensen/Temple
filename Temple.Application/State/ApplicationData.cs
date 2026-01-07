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

    public IQuestTree QuestTree { get; }

    public ApplicationData(
        IQuestTree questTree)
    {
        QuestTree = questTree;
        Party = new List<Creature>();
        BattlesWon = new HashSet<string>();
    }

    public void Reset()
    {
        QuestTree.Reset();
        Party = new List<Creature>();
        BattlesWon = new HashSet<string>();
    }
}

