using Temple.Domain.Entities.PR;

namespace Temple.Domain
{
    public static class PersonCommentExtensions
    {
        public static PersonComment Clone(
            this PersonComment personComment)
        {
            var clone = new PersonComment();
            clone.CopyAttributes(personComment);
            return clone;
        }

        public static void CopyAttributes(
            this PersonComment person,
            PersonComment other)
        {
            person.ArchiveID = other.ArchiveID;
            person.PersonID = other.PersonID;
            person.PersonArchiveID = other.PersonArchiveID;
            person.Created = other.Created;
            person.Superseded = other.Superseded;
            person.Start = other.Start;
            person.End = other.End;
            person.ID = other.ID;
            person.Text = other.Text;
        }
    }
}