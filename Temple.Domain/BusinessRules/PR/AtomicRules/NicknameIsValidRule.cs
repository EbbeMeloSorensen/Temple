using Craft.Domain;
using Temple.Domain.Entities.PR;

namespace Temple.Domain.BusinessRules.PR.AtomicRules
{
    public class NicknameIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "Nickname";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            Person person)
        {
            if (!string.IsNullOrEmpty(person.Nickname) && person.Nickname.Length > 20)
            {
                ErrorMessage = "Nicjname too long (max 20 characters)";
                return false;
            }

            return true;
        }
    }
}