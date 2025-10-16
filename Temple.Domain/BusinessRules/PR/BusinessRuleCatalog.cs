using Craft.Domain;
using Temple.Domain.BusinessRules.PR.AtomicRules;
using Temple.Domain.BusinessRules.PR.CrossEntityRules;

namespace Temple.Domain.BusinessRules.PR
{
    public class BusinessRuleCatalog : BusinessRuleCatalogBase
    {
        public BusinessRuleCatalog()
        {
            // Temporarily commented out to make sure the rule is enforced by the repository
            RegisterAtomicRule(new FirstNameIsValidRule());
            RegisterAtomicRule(new SurnameIsValidRule());
            RegisterAtomicRule(new NicknameIsValidRule());
            RegisterAtomicRule(new AddressIsValidRule());
            RegisterAtomicRule(new ZipCodeIsValidRule());
            RegisterAtomicRule(new CityIsValidRule());
            RegisterAtomicRule(new CategoryIsValidRule());
            RegisterAtomicRule(new BirthdayIsValidRule());
            RegisterAtomicRule(new DateRangeIsValidRule());

            RegisterCrossEntityRule(new DateRangeCollectionRule());
            //RegisterCrossEntityRule(new BirthdayConsistencyRule());
        }
    }
}
