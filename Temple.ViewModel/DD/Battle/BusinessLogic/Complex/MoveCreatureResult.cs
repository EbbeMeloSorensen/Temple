using Temple.Domain.Entities.DD.Battle;

namespace Temple.ViewModel.DD.Battle.BusinessLogic.Complex
{
    public class MoveCreatureResult
    {
        public int? IndexOfDestinationSquare { get; set; }
        public double? WalkingDistanceToDestinationSquare { get; set; }
        public Creature FinalClosestOpponent { get; set; }
        public double FinalDistanceToClosestOpponent { get; set; }
    }
}
