namespace Temple.Domain.Entities.DD.Exploration
{
    public class NPC : SiteComponent_Rotatable
    {
        public string Tag { get; set; }

        public NPC(
            string modelId) : base(modelId)
        {
        }
    }
}
