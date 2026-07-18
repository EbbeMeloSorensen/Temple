using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration;

public interface ISiteComponent
{
    // Condition determining if the NPC should be included in the scene
    public IGameCondition? Condition { get; set; }
}

