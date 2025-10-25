﻿using Stateless;
using Temple.Application.State;

namespace Temple.Application.Interfaces;

public interface IStateMachineIO
{
    void ExportTheDamnThing(
        StateMachine<ApplicationState, ApplicationTrigger> stateMachine);
}