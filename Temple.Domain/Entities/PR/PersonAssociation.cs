using Craft.Domain;

namespace Temple.Domain.Entities.PR
{
    public class PersonAssociation : IObjectWithGuidID, IObjectWithValidTime
    {
        public Guid ArchiveID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid ID { get; set; }

        public Guid SubjectPersonID { get; set; }
        public Guid SubjectPersonArchiveID { get; set; }
        public Person SubjectPerson { get; set; }

        public Guid ObjectPersonID { get; set; }
        public Guid ObjectPersonArchiveID { get; set; }
        public Person ObjectPerson { get; set; }

        public string Text { get; set; }
    }
}
