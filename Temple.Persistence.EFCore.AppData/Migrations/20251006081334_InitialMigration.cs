using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Temple.Persistence.EFCore.AppData.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoordinateSystems",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoordinateSystems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ObjectItems",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AlternativeIdentificationText = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectItems", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    ArchiveID = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Superseded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: true),
                    Nickname = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    ZipCode = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    Birthday = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Dead = table.Column<bool>(type: "boolean", nullable: true),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.ArchiveID);
                });

            migrationBuilder.CreateTable(
                name: "Smurfs",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Smurfs", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "VerticalDistances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Dimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerticalDistances", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lines_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Points",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Points", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Points_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Surfaces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surfaces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Surfaces_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Organisations_ObjectItems_ID",
                        column: x => x.ID,
                        principalTable: "ObjectItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonAssociations",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    ArchiveID = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Superseded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubjectPersonID = table.Column<Guid>(type: "uuid", nullable: false),
                    SubjectPersonArchiveID = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectPersonID = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjectPersonArchiveID = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAssociations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PersonAssociations_People_ObjectPersonArchiveID",
                        column: x => x.ObjectPersonArchiveID,
                        principalTable: "People",
                        principalColumn: "ArchiveID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonAssociations_People_SubjectPersonArchiveID",
                        column: x => x.SubjectPersonArchiveID,
                        principalTable: "People",
                        principalColumn: "ArchiveID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PersonComments",
                columns: table => new
                {
                    ArchiveID = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Superseded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Start = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    End = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonID = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonArchiveID = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonComments", x => x.ArchiveID);
                    table.ForeignKey(
                        name: "FK_PersonComments_People_PersonArchiveID",
                        column: x => x.PersonArchiveID,
                        principalTable: "People",
                        principalColumn: "ArchiveID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeometricVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LowerVerticalDistanceID = table.Column<Guid>(type: "uuid", nullable: true),
                    UpperVerticalDistanceID = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeometricVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeometricVolumes_Locations_Id",
                        column: x => x.Id,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeometricVolumes_VerticalDistances_LowerVerticalDistanceID",
                        column: x => x.LowerVerticalDistanceID,
                        principalTable: "VerticalDistances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeometricVolumes_VerticalDistances_UpperVerticalDistanceID",
                        column: x => x.UpperVerticalDistanceID,
                        principalTable: "VerticalDistances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AbsolutePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LatitudeCoordinate = table.Column<double>(type: "double precision", nullable: false),
                    LongitudeCoordinate = table.Column<double>(type: "double precision", nullable: false),
                    VerticalDistanceId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsolutePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbsolutePoints_Points_Id",
                        column: x => x.Id,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbsolutePoints_VerticalDistances_VerticalDistanceId",
                        column: x => x.VerticalDistanceId,
                        principalTable: "VerticalDistances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LinePoints",
                columns: table => new
                {
                    LineID = table.Column<Guid>(type: "uuid", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    PointId = table.Column<Guid>(type: "uuid", nullable: false),
                    SequenceQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinePoints", x => new { x.LineID, x.Index });
                    table.ForeignKey(
                        name: "FK_LinePoints_Lines_LineID",
                        column: x => x.LineID,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LinePoints_Points_PointId",
                        column: x => x.PointId,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PointReferences",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    XVectorPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    YVectorPointID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointReferences", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PointReferences_CoordinateSystems_ID",
                        column: x => x.ID,
                        principalTable: "CoordinateSystems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PointReferences_Points_OriginPointID",
                        column: x => x.OriginPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointReferences_Points_XVectorPointID",
                        column: x => x.XVectorPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PointReferences_Points_YVectorPointID",
                        column: x => x.YVectorPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RelativePoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoordinateSystemID = table.Column<Guid>(type: "uuid", nullable: false),
                    XCoordinateDimension = table.Column<double>(type: "double precision", nullable: false),
                    YCoordinateDimension = table.Column<double>(type: "double precision", nullable: false),
                    ZCoordinateDimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RelativePoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RelativePoints_CoordinateSystems_CoordinateSystemID",
                        column: x => x.CoordinateSystemID,
                        principalTable: "CoordinateSystems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RelativePoints_Points_Id",
                        column: x => x.Id,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CorridorAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CenterLineID = table.Column<Guid>(type: "uuid", nullable: false),
                    WidthDimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorridorAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CorridorAreas_Lines_CenterLineID",
                        column: x => x.CenterLineID,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CorridorAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ellipses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CentrePointID = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstConjugateDiameterPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    SecondConjugateDiameterPointID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ellipses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ellipses_Points_CentrePointID",
                        column: x => x.CentrePointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipses_Points_FirstConjugateDiameterPointID",
                        column: x => x.FirstConjugateDiameterPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipses_Points_SecondConjugateDiameterPointID",
                        column: x => x.SecondConjugateDiameterPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ellipses_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FanAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VertexPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    MinimumRangeDimension = table.Column<double>(type: "double precision", nullable: false),
                    MaximumRangeDimension = table.Column<double>(type: "double precision", nullable: false),
                    OrientationAngle = table.Column<double>(type: "double precision", nullable: false),
                    SectorSizeAngle = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FanAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FanAreas_Points_VertexPointID",
                        column: x => x.VertexPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FanAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrbitAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    SecondPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    OrbitAreaAlignmentCode = table.Column<int>(type: "integer", nullable: false),
                    WidthDimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrbitAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrbitAreas_Points_FirstPointID",
                        column: x => x.FirstPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrbitAreas_Points_SecondPointID",
                        column: x => x.SecondPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrbitAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolyArcAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefiningLineID = table.Column<Guid>(type: "uuid", nullable: false),
                    BearingOriginPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    BeginBearingAngle = table.Column<double>(type: "double precision", nullable: false),
                    EndBearingAngle = table.Column<double>(type: "double precision", nullable: false),
                    ArcRadiusDimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolyArcAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolyArcAreas_Lines_DefiningLineID",
                        column: x => x.DefiningLineID,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolyArcAreas_Points_BearingOriginPointID",
                        column: x => x.BearingOriginPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolyArcAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PolygonAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoundingLineID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PolygonAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PolygonAreas_Lines_BoundingLineID",
                        column: x => x.BoundingLineID,
                        principalTable: "Lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PolygonAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackAreas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BeginPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    EndPointID = table.Column<Guid>(type: "uuid", nullable: false),
                    LeftWidthDimension = table.Column<double>(type: "double precision", nullable: false),
                    RightWidthDimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackAreas_Points_BeginPointID",
                        column: x => x.BeginPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackAreas_Points_EndPointID",
                        column: x => x.EndPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrackAreas_Surfaces_Id",
                        column: x => x.Id,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    FormalAbbreviatedName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Units_Organisations_ID",
                        column: x => x.ID,
                        principalTable: "Organisations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConeVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefiningSurfaceID = table.Column<Guid>(type: "uuid", nullable: false),
                    VertexPointID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConeVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConeVolumes_GeometricVolumes_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConeVolumes_Points_VertexPointID",
                        column: x => x.VertexPointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConeVolumes_Surfaces_DefiningSurfaceID",
                        column: x => x.DefiningSurfaceID,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SphereVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CentrePointID = table.Column<Guid>(type: "uuid", nullable: false),
                    RadiusDimension = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SphereVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SphereVolumes_GeometricVolumes_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SphereVolumes_Points_CentrePointID",
                        column: x => x.CentrePointID,
                        principalTable: "Points",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SurfaceVolumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefiningSurfaceID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurfaceVolumes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurfaceVolumes_GeometricVolumes_Id",
                        column: x => x.Id,
                        principalTable: "GeometricVolumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SurfaceVolumes_Surfaces_DefiningSurfaceID",
                        column: x => x.DefiningSurfaceID,
                        principalTable: "Surfaces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbsolutePoints_VerticalDistanceId",
                table: "AbsolutePoints",
                column: "VerticalDistanceId");

            migrationBuilder.CreateIndex(
                name: "IX_ConeVolumes_DefiningSurfaceID",
                table: "ConeVolumes",
                column: "DefiningSurfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_ConeVolumes_VertexPointID",
                table: "ConeVolumes",
                column: "VertexPointID");

            migrationBuilder.CreateIndex(
                name: "IX_CorridorAreas_CenterLineID",
                table: "CorridorAreas",
                column: "CenterLineID");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipses_CentrePointID",
                table: "Ellipses",
                column: "CentrePointID");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipses_FirstConjugateDiameterPointID",
                table: "Ellipses",
                column: "FirstConjugateDiameterPointID");

            migrationBuilder.CreateIndex(
                name: "IX_Ellipses_SecondConjugateDiameterPointID",
                table: "Ellipses",
                column: "SecondConjugateDiameterPointID");

            migrationBuilder.CreateIndex(
                name: "IX_FanAreas_VertexPointID",
                table: "FanAreas",
                column: "VertexPointID");

            migrationBuilder.CreateIndex(
                name: "IX_GeometricVolumes_LowerVerticalDistanceID",
                table: "GeometricVolumes",
                column: "LowerVerticalDistanceID");

            migrationBuilder.CreateIndex(
                name: "IX_GeometricVolumes_UpperVerticalDistanceID",
                table: "GeometricVolumes",
                column: "UpperVerticalDistanceID");

            migrationBuilder.CreateIndex(
                name: "IX_LinePoints_PointId",
                table: "LinePoints",
                column: "PointId");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitAreas_FirstPointID",
                table: "OrbitAreas",
                column: "FirstPointID");

            migrationBuilder.CreateIndex(
                name: "IX_OrbitAreas_SecondPointID",
                table: "OrbitAreas",
                column: "SecondPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAssociations_ObjectPersonArchiveID",
                table: "PersonAssociations",
                column: "ObjectPersonArchiveID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAssociations_SubjectPersonArchiveID",
                table: "PersonAssociations",
                column: "SubjectPersonArchiveID");

            migrationBuilder.CreateIndex(
                name: "IX_PersonComments_PersonArchiveID",
                table: "PersonComments",
                column: "PersonArchiveID");

            migrationBuilder.CreateIndex(
                name: "IX_PointReferences_OriginPointID",
                table: "PointReferences",
                column: "OriginPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PointReferences_XVectorPointID",
                table: "PointReferences",
                column: "XVectorPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PointReferences_YVectorPointID",
                table: "PointReferences",
                column: "YVectorPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PolyArcAreas_BearingOriginPointID",
                table: "PolyArcAreas",
                column: "BearingOriginPointID");

            migrationBuilder.CreateIndex(
                name: "IX_PolyArcAreas_DefiningLineID",
                table: "PolyArcAreas",
                column: "DefiningLineID");

            migrationBuilder.CreateIndex(
                name: "IX_PolygonAreas_BoundingLineID",
                table: "PolygonAreas",
                column: "BoundingLineID");

            migrationBuilder.CreateIndex(
                name: "IX_RelativePoints_CoordinateSystemID",
                table: "RelativePoints",
                column: "CoordinateSystemID");

            migrationBuilder.CreateIndex(
                name: "IX_SphereVolumes_CentrePointID",
                table: "SphereVolumes",
                column: "CentrePointID");

            migrationBuilder.CreateIndex(
                name: "IX_SurfaceVolumes_DefiningSurfaceID",
                table: "SurfaceVolumes",
                column: "DefiningSurfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAreas_BeginPointID",
                table: "TrackAreas",
                column: "BeginPointID");

            migrationBuilder.CreateIndex(
                name: "IX_TrackAreas_EndPointID",
                table: "TrackAreas",
                column: "EndPointID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsolutePoints");

            migrationBuilder.DropTable(
                name: "ConeVolumes");

            migrationBuilder.DropTable(
                name: "CorridorAreas");

            migrationBuilder.DropTable(
                name: "Ellipses");

            migrationBuilder.DropTable(
                name: "FanAreas");

            migrationBuilder.DropTable(
                name: "LinePoints");

            migrationBuilder.DropTable(
                name: "OrbitAreas");

            migrationBuilder.DropTable(
                name: "PersonAssociations");

            migrationBuilder.DropTable(
                name: "PersonComments");

            migrationBuilder.DropTable(
                name: "PointReferences");

            migrationBuilder.DropTable(
                name: "PolyArcAreas");

            migrationBuilder.DropTable(
                name: "PolygonAreas");

            migrationBuilder.DropTable(
                name: "RelativePoints");

            migrationBuilder.DropTable(
                name: "Smurfs");

            migrationBuilder.DropTable(
                name: "SphereVolumes");

            migrationBuilder.DropTable(
                name: "SurfaceVolumes");

            migrationBuilder.DropTable(
                name: "TrackAreas");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Lines");

            migrationBuilder.DropTable(
                name: "CoordinateSystems");

            migrationBuilder.DropTable(
                name: "GeometricVolumes");

            migrationBuilder.DropTable(
                name: "Points");

            migrationBuilder.DropTable(
                name: "Surfaces");

            migrationBuilder.DropTable(
                name: "Organisations");

            migrationBuilder.DropTable(
                name: "VerticalDistances");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "ObjectItems");
        }
    }
}
