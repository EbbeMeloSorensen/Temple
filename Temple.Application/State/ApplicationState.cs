namespace Temple.Application.State;

public record ApplicationState(
    ApplicationStateType Type,
    ApplicationStatePayload? Payload = null);