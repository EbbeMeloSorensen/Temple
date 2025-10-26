namespace Temple.Domain.Entities.DD
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
