namespace Temple.Domain.Entities.C2IEDM.ObjectItems
{
    public class Organisation : ObjectItem
    {
        public string? NickName { get; set; }

        public Organisation()
        {
            NickName = "";
        }
    }
}
