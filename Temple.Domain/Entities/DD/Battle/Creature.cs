namespace Temple.Domain.Entities.DD.Battle
{
    public class Creature : BoardItem
    {
        private CreatureType _creatureType;
        private Queue<Attack> _attacks;

        public CreatureType CreatureType
        {
            get { return _creatureType; }
            private set { _creatureType = value; }
        }

        public Queue<Attack> Attacks
        {
            get { return _attacks; }
            set { _attacks = value; }
        }

        public bool IsHostile { get; set; }
        public int HitPoints { get; set; }
        public bool IsAutomatic { get; set; }
        public int BattleRoundQueueNumber { get; set; }

        public Creature(
            CreatureType creatureType,
            bool isHostile)
        {
            CreatureType = creatureType;
            HitPoints = CreatureType.MaxHitPoints;
            IsHostile = isHostile;
            IsAutomatic = IsHostile;
            Attacks = new Queue<Attack>();
        }

        public override string ToString()
        {
            return CreatureType.Name;
        }
    }
}
