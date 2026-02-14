namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public interface IDialogueGraphCondition
{
    bool Evaluate(
        IDialogueQueryService query);
}