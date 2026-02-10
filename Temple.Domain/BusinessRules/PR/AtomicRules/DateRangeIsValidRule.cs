using Craft.Domain;
using Temple.Domain.Entities.PR;

namespace Temple.Domain.BusinessRules.PR.AtomicRules
{
    public class DateRangeIsValidRule : IBusinessRule<Person>
    {
        private static readonly DateTime _maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        public string RuleName => "DateRange";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            Person person)
        {
            if (person.Start == default)
            {
                ErrorMessage = "Start is required";
                return false;
            }

            if (person.Start >= person.End)
            {
                ErrorMessage = "End must exceed Start";
                return false;
            }

            var now = DateTime.UtcNow;

            if (person.Start > now ||
                person.End != _maxDateTime && person.End > now)
            {
                //ErrorMessage = "These time entries cannot be in the future";
                ErrorMessage = "Oops! Make sure these time entries are not in the future";
                return false;
            }

            return true;
        }
    }
}