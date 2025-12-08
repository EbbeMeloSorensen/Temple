namespace Temple.Application.State.Payloads;

public class BattlePayload : ApplicationStatePayload
{
    public string BattleId { get; set; }
    public string? EntranceId { get; set; }
    public ApplicationStatePayload PayloadForNextStateInCasePartyWins { get; set; }
}