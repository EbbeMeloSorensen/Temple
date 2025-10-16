namespace Temple.Persistence.EFCore.Dummies
{
    public class Dummy
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public Dummy()
        {
            Name = "";
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
