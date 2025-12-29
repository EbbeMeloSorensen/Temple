namespace Temple.Domain.Entities.DD.Battle
{
    public abstract class Attack
    {
        public string Name { get; set; }
        public int MaxDamage { get; set; }

        protected Attack(
            string name,
            int maxDamage)
        {
            Name = name;
            MaxDamage = maxDamage;
        }
    }
}
