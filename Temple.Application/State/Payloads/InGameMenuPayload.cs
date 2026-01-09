namespace Temple.Application.State.Payloads;

public class InGameMenuPayload : ApplicationStatePayload
{
    public ApplicationStatePayload PayloadForNextState { get; set; }
}