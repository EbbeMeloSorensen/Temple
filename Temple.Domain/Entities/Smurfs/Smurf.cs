namespace Temple.Domain.Entities.Smurfs
{
    public class Smurf
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public SmurfRole Role { get; set; } = SmurfRole.Unspecified;

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
