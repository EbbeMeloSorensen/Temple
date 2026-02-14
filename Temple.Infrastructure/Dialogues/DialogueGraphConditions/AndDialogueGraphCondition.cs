using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class AndDialogueGraphCondition : IDialogueGraphCondition
{
    public List<IDialogueGraphCondition> Conditions { get; } = new();

    public bool Evaluate(
        IDialogueQueryService query)
    {
        return Conditions.All(c => c.Evaluate(query));
    }
}