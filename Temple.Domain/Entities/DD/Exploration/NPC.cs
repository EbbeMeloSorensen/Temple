using Craft.Math;
using Temple.Domain.Entities.DD.Common;

namespace Temple.Domain.Entities.DD.Exploration
{
    public class NPC : ISiteComponent_Rotatable
    {
        public IGameCondition? Condition { get; set; }

        public Vector3D Position { get; set; }

        public double Orientation { get; set; }

        public string ModelId { get; init; }

        public string Id { get; set; }
    }
}
