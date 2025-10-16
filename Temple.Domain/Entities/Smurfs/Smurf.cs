namespace Temple.Domain.Entities.Smurfs
{
    public class Smurf
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public Smurf()
        {
            Name = "";
        }

        public override string ToString()
        {
            return $"{Name}";
        }
    }
}
