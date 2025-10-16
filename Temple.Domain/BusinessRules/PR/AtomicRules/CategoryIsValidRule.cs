using Craft.Domain;
using Temple.Domain.Entities.PR;

namespace Temple.Domain.BusinessRules.PR.AtomicRules
{
    public class CategoryIsValidRule : IBusinessRule<Person>
    {
        public string RuleName => "Category";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            Person person)
        {
            if (!string.IsNullOrEmpty(person.Category) && person.Category.Length > 20)
            {
                ErrorMessage = "Category too long (max 20 characters)";
                return false;
            }

            return true;
        }
    }
}