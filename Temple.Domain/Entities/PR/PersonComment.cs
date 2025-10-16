using System;
using Craft.Domain;

namespace Temple.Domain.Entities.PR
{
    public class PersonComment : IObjectWithGuidID, IObjectWithValidTime
    {
        public Guid ArchiveID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid ID { get; set; }

        public Guid PersonID { get; set; }
        public Guid PersonArchiveID { get; set; }
        public Person Person { get; set; }

        public string Text { get; set; }

        public override string ToString()
        {
            return $"{Text}";
        }
    }
}
