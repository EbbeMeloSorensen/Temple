namespace Temple.Domain.Entities.DD.Exploration
{
    public class NPC : SiteComponent_Rotatable
    {
        public string Name { get; set; }

        public NPC(
            string modelId) : base(modelId)
        {
        }
    }
}
