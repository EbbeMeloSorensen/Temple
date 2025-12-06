namespace Temple.Application.State;

public class BattlePayload : ApplicationStatePayload
{
    public string BattleId { get; set; }
    public ApplicationStatePayload PayloadForNextStateInCasePartyWins { get; set; }
}