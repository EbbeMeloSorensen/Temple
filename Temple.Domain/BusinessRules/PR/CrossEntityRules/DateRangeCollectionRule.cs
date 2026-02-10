using Craft.Domain;
using Craft.Math;
using Temple.Domain.Entities.PR;

namespace Temple.Domain.BusinessRules.PR.CrossEntityRules
{
    public class DateRangeCollectionRule : IBusinessRule<IEnumerable<Person>>
    {
        public string RuleName => "ValidTimeIntervals";

        public string ErrorMessage { get; private set; } = "";

        public bool Validate(
            IEnumerable<Person> variants)
        {
            var validTimeIntervals = variants
                .Select(_ => new Tuple<DateTime, DateTime>(_.Start, _.End))
                .OrderBy(_ => _.Item1)
                .ToList();

            if (validTimeIntervals.AnyOverlaps())
            {
                ErrorMessage = "Valid time intervals overlapping";
                return false;
            }

            if (validTimeIntervals.AnyGaps())
            {
                ErrorMessage = "Gaps between valid time intervals";
                return false;
            }

            return true;
        }
    }
}