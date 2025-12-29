namespace Temple.Domain.Entities.DD.Exploration;

public class EventTrigger_ScriptedBattle : EventTrigger
{
    public string? EntranceID { get; set; }

    public EventTrigger_ScriptedBattle(
        string modelId) : base(modelId)
    {
    }
}