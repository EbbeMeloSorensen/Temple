using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public class EventTrigger_ScriptedBattle : IEventTrigger
{
    public IGameCondition? Condition { get; set; }


    public Point2D Point1 { get; set; }

    public Point2D Point2 { get; set; }

    public string EventID { get; set; }

    public int? QuestID { get; set; }

    public string? EntranceID { get; set; }
}