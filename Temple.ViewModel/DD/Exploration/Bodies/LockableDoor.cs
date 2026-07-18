using Craft.Simulation.Bodies;
using Temple.Domain.Entities.DD.Common;

namespace Temple.ViewModel.DD.Exploration.Bodies
{
    public class LockableDoor : BodyDoor
    {
        public IGameCondition ConditionForAccessibility { get; set; }

        public LockableDoor(
            int id,
            double mass,
            bool affectedByGravity,
            bool affectedByBoundaries,
            string? tag) : base(
                id,
                mass,
                affectedByGravity,
                affectedByBoundaries, tag)
        {
        }
    }
}
