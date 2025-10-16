namespace Temple.Domain.Entities.C2IEDM.Affiliations
{
    public enum AffiliationEthnicGroupCode
    {
        AustralianAboriginal,
        Flemish,
        Hawaiian,
        Kurd,
        NewZealand,
        Maori,
        Sikh,
        Tamil,
        Welsh
    }

    public class AffiliationEthnicGroup : Affiliation
    {
        public AffiliationEthnicGroupCode AffiliationEthnicGroupCode { get; set; }
    }
}
