using Microsoft.EntityFrameworkCore;
using Temple.Domain.Entities.C2IEDM.Geometry;
using Temple.Domain.Entities.C2IEDM.ObjectItems;
using Temple.Domain.Entities.PR;
using Temple.Domain.Entities.Smurfs;
using Temple.Persistence.EFCore.AppData.EntityConfigurations.C2IEDM.Geometry.Locations.Line;
using Temple.Persistence.EFCore.AppData.EntityConfigurations.PR;

namespace Temple.Persistence.EFCore.AppData
{
    public class PRDbContextBase : DbContext
    {
        public static bool Versioned { get; set; }

        // This constructor is necessary when making a migration with the Package Manager console, since we don't set the static property Versioned when
        // making a migration
        static PRDbContextBase()
        {
            Versioned = true;
        }

        public PRDbContextBase(
            DbContextOptions<PRDbContextBase> options) : base(options)
        {
        }

        public DbSet<Smurf> Smurfs { get; set; }

        public DbSet<Domain.Entities.PR.Person> People { get; set; }
        public DbSet<PersonComment> PersonComments { get; set; }
        public DbSet<PersonAssociation> PersonAssociations { get; set; }

        // C2IEDM - Geometry
        public DbSet<AbsolutePoint> AbsolutePoints { get; set; }
        public DbSet<ConeVolume> ConeVolumes { get; set; }
        public DbSet<CoordinateSystem> CoordinateSystems { get; set; }
        public DbSet<CorridorArea> CorridorAreas { get; set; }
        public DbSet<Ellipse> Ellipses { get; set; }
        public DbSet<FanArea> FanAreas { get; set; }
        public DbSet<GeometricVolume> GeometricVolumes { get; set; }
        public DbSet<LinePoint> LinePoints { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<OrbitArea> OrbitAreas { get; set; }
        public DbSet<PointReference> PointReferences { get; set; }
        public DbSet<Point> Points { get; set; }
        public DbSet<PolyArcArea> PolyArcAreas { get; set; }
        public DbSet<PolygonArea> PolygonAreas { get; set; }
        public DbSet<RelativePoint> RelativePoints { get; set; }
        public DbSet<SphereVolume> SphereVolumes { get; set; }
        public DbSet<Surface> Surfaces { get; set; }
        public DbSet<SurfaceVolume> SurfaceVolumes { get; set; }
        public DbSet<TrackArea> TrackAreas { get; set; }
        public DbSet<VerticalDistance> VerticalDistances { get; set; }

        // C2IEDM - ObjectItems
        public DbSet<ObjectItem> ObjectItems { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            Configure(modelBuilder);
        }

        public static void Configure(
            ModelBuilder modelBuilder)
        {
            ConfigureC2IEDM(modelBuilder);

            modelBuilder.ApplyConfiguration(new PersonConfiguration(Versioned));
            modelBuilder.ApplyConfiguration(new PersonCommentConfiguration(Versioned));

            if (Versioned)
            {
                modelBuilder.Entity<PersonComment>()
                    .HasOne(pc => pc.Person)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(pc => pc.PersonArchiveID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<PersonAssociation>()
                    .HasOne(pa => pa.SubjectPerson)
                    .WithMany(p => p.ObjectPeople)
                    .HasForeignKey(pa => pa.SubjectPersonArchiveID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<PersonAssociation>()
                    .HasOne(pa => pa.ObjectPerson)
                    .WithMany(p => p.SubjectPeople)
                    .HasForeignKey(pa => pa.ObjectPersonArchiveID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            }
            else
            {
                modelBuilder.Entity<PersonComment>()
                    .HasOne(pc => pc.Person)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(pc => pc.PersonID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<PersonAssociation>()
                    .HasOne(pa => pa.SubjectPerson)
                    .WithMany(p => p.ObjectPeople)
                    .HasForeignKey(pa => pa.SubjectPersonID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);

                modelBuilder.Entity<PersonAssociation>()
                    .HasOne(pa => pa.ObjectPerson)
                    .WithMany(p => p.SubjectPeople)
                    .HasForeignKey(pa => pa.ObjectPersonID)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }

        private static void ConfigureC2IEDM(
            ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new LinePointConfiguration());

            modelBuilder.Entity<Location>().UseTptMappingStrategy();
            modelBuilder.Entity<CoordinateSystem>().UseTptMappingStrategy();
            modelBuilder.Entity<ObjectItem>().UseTptMappingStrategy();

            modelBuilder.Entity<AbsolutePoint>()
                .HasOne(ap => ap.VerticalDistance)
                .WithMany()
                .HasForeignKey(ap => ap.VerticalDistanceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConeVolume>()
                .HasOne(cv => cv.VertexPoint)
                .WithMany()
                .HasForeignKey(cv => cv.VertexPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ConeVolume>()
                .HasOne(cv => cv.DefiningSurface)
                .WithMany()
                .HasForeignKey(cv => cv.DefiningSurfaceID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CorridorArea>()
                .HasOne(ca => ca.CenterLine)
                .WithMany()
                .HasForeignKey(ca => ca.CenterLineID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ellipse>()
                .HasOne(e => e.CentrePoint)
                .WithMany()
                .HasForeignKey(e => e.CentrePointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ellipse>()
                .HasOne(e => e.FirstConjugateDiameterPoint)
                .WithMany()
                .HasForeignKey(e => e.FirstConjugateDiameterPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ellipse>()
                .HasOne(e => e.SecondConjugateDiameterPoint)
                .WithMany()
                .HasForeignKey(e => e.SecondConjugateDiameterPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FanArea>()
                .HasOne(fa => fa.VertexPoint)
                .WithMany()
                .HasForeignKey(fa => fa.VertexPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GeometricVolume>()
                .HasOne(gv => gv.LowerVerticalDistance)
                .WithMany()
                .HasForeignKey(gv => gv.LowerVerticalDistanceID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GeometricVolume>()
                .HasOne(gv => gv.UpperVerticalDistance)
                .WithMany()
                .HasForeignKey(gv => gv.UpperVerticalDistanceID)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Line>()
                .HasMany(l => l.LinePoints)
                .WithOne(lp => lp.Line)
                .HasForeignKey(lp => lp.LineID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PointReference>()
                .HasOne(pr => pr.OriginPoint)
                .WithMany()
                .HasForeignKey(pr => pr.OriginPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PointReference>()
                .HasOne(pr => pr.XVectorPoint)
                .WithMany()
                .HasForeignKey(pr => pr.XVectorPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PointReference>()
                .HasOne(pr => pr.YVectorPoint)
                .WithMany()
                .HasForeignKey(pr => pr.YVectorPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PolygonArea>()
                .HasOne(pa => pa.BoundingLine)
                .WithMany()
                .HasForeignKey(pa => pa.BoundingLineID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RelativePoint>()
                .HasOne(rp => rp.CoordinateSystem)
                .WithMany()
                .HasForeignKey(rp => rp.CoordinateSystemID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SphereVolume>()
                .HasOne(sv => sv.CentrePoint)
                .WithMany()
                .HasForeignKey(sv => sv.CentrePointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurfaceVolume>()
                .HasOne(sv => sv.DefiningSurface)
                .WithMany()
                .HasForeignKey(sv => sv.DefiningSurfaceID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrbitArea>()
                .HasOne(oa => oa.FirstPoint)
                .WithMany()
                .HasForeignKey(oa => oa.FirstPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrbitArea>()
                .HasOne(oa => oa.SecondPoint)
                .WithMany()
                .HasForeignKey(oa => oa.SecondPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PolyArcArea>()
                .HasOne(paa => paa.DefiningLine)
                .WithMany()
                .HasForeignKey(paa => paa.DefiningLineID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PolyArcArea>()
                .HasOne(paa => paa.BearingOriginPoint)
                .WithMany()
                .HasForeignKey(paa => paa.BearingOriginPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrackArea>()
                .HasOne(ta => ta.BeginPoint)
                .WithMany()
                .HasForeignKey(ta => ta.BeginPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TrackArea>()
                .HasOne(ta => ta.EndPoint)
                .WithMany()
                .HasForeignKey(ta => ta.EndPointID)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
