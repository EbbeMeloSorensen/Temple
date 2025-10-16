namespace Temple.Domain.Entities.C2IEDM.ObjectItems
{
    public class Unit : Organisation
    {
        public string FormalAbbreviatedName { get; set; }

        public Unit()
        {
            FormalAbbreviatedName = "";
        }
    }
}
