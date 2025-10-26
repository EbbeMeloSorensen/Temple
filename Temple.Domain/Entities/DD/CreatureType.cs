namespace Temple.Domain.Entities.DD
{
    public class CreatureType
    {
        public Guid ID { get; set; }

        private List<Attack> _attacks;

        public List<Attack> Attacks
        {
            get { return _attacks; }
            set { _attacks = value; }
        }

        public string Name { get; set; }
        public int MaxHitPoints { get; set; }
        public int ArmorClass { get; set; }
        public int Thaco { get; set; }
        public int InitiativeModifier { get; set; }
        public double Movement { get; set; }

        public CreatureType()
        {
            Attacks = new List<Attack>();
        }

        public CreatureType(
            string name,
            int maxHitPoints,
            int armorClass,
            int thaco,
            int initiativeModifier,
            double movement,
            List<Attack> attacks)
        {
            Name = name;
            MaxHitPoints = maxHitPoints;
            ArmorClass = armorClass;
            Thaco = thaco;
            InitiativeModifier = initiativeModifier;
            Movement = movement;
            Attacks = attacks;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
