using System;

namespace Temple.Domain.Entities.WMDR
{
    public abstract class GeospatialLocation
    {
        public Guid ID { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public Guid AbstractEnvironmentalMonitoringFacilityID { get; set; }
        public Guid AbstractEnvironmentalMonitoringFacilityArchiveID { get; set; }
        public virtual AbstractEnvironmentalMonitoringFacility AbstractEnvironmentalMonitoringFacility { get; set; }
    }
}