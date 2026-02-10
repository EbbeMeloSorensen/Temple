using Craft.Domain;

namespace Temple.Domain.Entities.WMDR
{
    public abstract class AbstractEnvironmentalMonitoringFacility : IObjectWithValidTime, IVersionedObject
    {
        public Guid ArchiveID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Superseded { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public Guid ID { get; set; }
    }
}