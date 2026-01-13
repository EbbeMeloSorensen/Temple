using Craft.Math;

namespace Temple.Application.Interfaces;

public class QuestNew
{
    public QuestTrigger Trigger { get; set; }
    public IEnumerable<QuestStage> Stages { get; }

    public QuestNew(
        QuestTrigger trigger,
        IEnumerable<QuestStage> stages)
    {
        Trigger = trigger;
        Stages = stages;
    }
}

public abstract class QuestStage
{
}

public class DefeatEnemy : QuestStage
{
}

public class ReportBackToNPC : QuestStage
{
}

public abstract class QuestTrigger
{
}

public abstract class NPCDialogue : QuestTrigger
{
    public string ModelId { get; set; }
    public string NPCName { get; set; }
    public Point2D Position { get; set; }
    public double Orientation { get; set; }
    public double Height { get; set; }
}