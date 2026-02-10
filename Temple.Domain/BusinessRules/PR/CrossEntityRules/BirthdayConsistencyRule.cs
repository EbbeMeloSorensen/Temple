using Craft.Domain;
using Temple.Domain.Entities.PR;

namespace Temple.Domain.BusinessRules.PR.CrossEntityRules
{
    public class BirthdayConsistencyRule : IBusinessRule<IEnumerable<Person>>
    {
        public string RuleName => "BirthdayConsistency";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            IEnumerable<Person> variants)
        {
            if (variants.All(_ => _.Birthday == null))
            {
                return true;
            }

            var birthday = variants.Select(_ => _.Birthday).Distinct().SingleOrDefault();

            if (!birthday.HasValue)
            {
                ErrorMessage = "Birthday is ambiguous";
                return false;
            }

            var startOfFirstValidTimeInterval = variants.Min(_ => _.Start);

            if (birthday.Value != startOfFirstValidTimeInterval)
            {
                ErrorMessage = "Birthday doesn't match start of oldest valid time interval";
                return false;
            }

            return true;
        }
    }
}