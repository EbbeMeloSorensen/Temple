using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public class EventTrigger_ScriptedBattle : IEventTrigger
{
    public Point2D Point1 { get; set; }
    public Point2D Point2 { get; set; }
    public string EventID { get; set; }
    public int? QuestID { get; set; }
    public string? EntranceID { get; set; }
}