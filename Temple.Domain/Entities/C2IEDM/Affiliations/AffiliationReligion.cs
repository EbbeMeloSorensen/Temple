namespace Temple.Domain.Entities.C2IEDM.Affiliations
{
    public enum AffiliationReligionCode
    {
        Anglican,
        Catholic,
        Hindu,
        Muslim
    }

    public class AffiliationReligion : Affiliation
    {
        public AffiliationReligionCode AffiliationReligionCode { get; set; }
    }
}
