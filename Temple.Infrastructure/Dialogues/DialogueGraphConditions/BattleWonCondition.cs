using Temple.Application.Interfaces;

namespace Temple.Infrastructure.Dialogues.DialogueGraphConditions;

public class BattleWonCondition : IDialogueGraphCondition
{
    public string BattleId { get; set; }

    public bool Evaluate(
        IDialogueQueryService query)
    {
        return query.IsBattleWon(BattleId);
    }
}