using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD.Battle.BusinessLogic.Complex
{
    public static class CreatureExtensions
    {
        public static Creature Clone(
            this Creature creature)
        {
            return new Creature(
                creature.CreatureType,
                creature.IsHostile)
            {
                PositionX = creature.PositionX,
                PositionY = creature.PositionY,
                HitPoints = creature.HitPoints,
                IsAutomatic = creature.IsAutomatic,
            };
        }
    }
}
