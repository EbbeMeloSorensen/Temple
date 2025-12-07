using Temple.Application.State.Payloads;

namespace Temple.Application.State;

public record ApplicationState(
    StateMachineState StateMachineState, // Afgør, hvilken view model, der skal aktiveres
    ApplicationStatePayload? Payload = null); // Bruges til intialisering af view modellen