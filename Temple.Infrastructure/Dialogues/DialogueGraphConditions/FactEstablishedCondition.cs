namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class FactEstablishedCondition : IDialogueGraphCondition
{
    public string FactId { get; set; }

    public bool Evaluate(
        IDialogueQueryService query)
    {
        throw new NotImplementedException();
    }
}