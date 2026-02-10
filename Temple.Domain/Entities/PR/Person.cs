using Craft.Domain;

namespace Temple.Domain.Entities.PR
{
    public class Person : IObjectWithGuidID, IObjectWithValidTime, IClonableObject
    {
        public Guid ArchiveID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid ID { get; set; }

        public string FirstName { get; set; }

        public string? Surname { get; set; }

        public string? Nickname { get; set; }

        public string? Address { get; set; }

        public string? ZipCode { get; set; }

        public string? City { get; set; }

        public DateTime? Birthday { get; set; }

        public string? Category { get; set; }

        public string? Description { get; set; }

        public bool? Dead { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public virtual ICollection<PersonComment>? Comments { get; set; }
        public virtual ICollection<PersonAssociation>? ObjectPeople { get; set; }
        public virtual ICollection<PersonAssociation>? SubjectPeople { get; set; }

        public IClonableObject Clone()
        {
            var clone = new Person();
            clone.CopyAttributes(this);
            return clone;
        }

        public override string ToString()
        {
            return $"{FirstName} ({Start} - {End})";
        }

        public void CopyAttributes(
            Person other)
        {
            ArchiveID = other.ArchiveID;
            Created = other.Created;
            Superseded = other.Superseded;
            Start = other.Start;
            End = other.End;
            ID = other.ID;
            FirstName = other.FirstName;
            Surname = other.Surname;
            Nickname = other.Nickname;
            Address = other.Address;
            ZipCode = other.ZipCode;
            City = other.City;
            Birthday = other.Birthday;
            Category = other.Category;
            Description = other.Description;
            Latitude = other.Latitude;
            Longitude = other.Longitude;
        }
    }
}
