namespace Temple.Application.State;

public abstract class ApplicationStatePayload
{
    public string JustAString { get; set; }

    public ApplicationStatePayload PayloadForNextState { get; set; }
}