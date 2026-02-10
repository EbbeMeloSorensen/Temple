namespace Temple.Domain.Entities.C2IEDM.ObjectItems

{
    public class ObjectItem
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string? AlternativeIdentificationText { get; set; }

        public ObjectItem()
        {
            Name = "";
        }
    }
}
