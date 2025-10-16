using Craft.Domain;
using Temple.Domain.Entities.PR;

namespace Temple.Domain.BusinessRules.PR.AtomicRules
{
    public class FirstNameIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "FirstName";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            Person person)
        {
            if (string.IsNullOrEmpty(person.FirstName))
            {
                ErrorMessage = "First name is required";
                return false;
            }

            if (person.FirstName.Length > 20)
            {
                ErrorMessage = "First name too long (max 20 characters)";
                return false;
            }

            return true;
        }
    }
}