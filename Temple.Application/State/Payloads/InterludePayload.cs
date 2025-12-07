namespace Temple.Application.State.Payloads;

public class InterludePayload : ApplicationStatePayload
{
    public string Text { get; set; }
    public ApplicationStatePayload PayloadForNextState { get; set; }
}