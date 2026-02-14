namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class BattleWonCondition : IDialogueGraphCondition
{
    public string BattleId { get; set; }

    public bool Evaluate(
        IDialogueQueryService query)
    {
        throw new NotImplementedException();
    }
}