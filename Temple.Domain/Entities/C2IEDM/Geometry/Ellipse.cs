namespace Temple.Domain.Entities.C2IEDM.Geometry
{
    public class Ellipse : Surface
    {
        public Guid CentrePointID { get; set; }
        public Point CentrePoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid FirstConjugateDiameterPointID { get; set; }
        public Point FirstConjugateDiameterPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)

        public Guid SecondConjugateDiameterPointID { get; set; }
        public Point SecondConjugateDiameterPoint { get; set; } //= null!; (Forstyrrer Enterprise Architect)
    }
}
