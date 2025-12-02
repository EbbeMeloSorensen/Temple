namespace Temple.Application.State;

public record ApplicationState(
    StateMachineState StateMachineState,
    StateMachineStateType? Type = null,
    ApplicationStatePayload? Payload = null);