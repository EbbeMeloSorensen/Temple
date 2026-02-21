using Craft.Math;
using FluentAssertions;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.UnitTest;

public class SiteDataIOTest
{
    [Fact]
    public void WriteSiteDataToFileThenReadItAndWriteItAgain()
    {
        // Arrange
        var fileName = @"C:\Temp\test_site_data.json";
        var siteData = new SiteData();

        var siteExtent = 20.0;

        siteData.AddQuad(
            new Point3D(siteExtent, siteExtent, 0),
            new Point3D(-siteExtent, siteExtent, 0),
            new Point3D(-siteExtent, -siteExtent, 0),
            new Point3D(siteExtent, -siteExtent, 0));

        siteData.AddWall(new List<Point2D>
        {
            new (1, 0),
            new (1, 2),
            new (2, 2)
        });

        // Act (Write)
        siteData.SiteComponents.WriteToFile(fileName);

        // Act (Read)
        var siteData2 = new SiteData
        {
            SiteComponents = SiteDataIO.ReadSiteComponentListFromFile(fileName).ToList()
        };

        // Act (Write the one that was read)
        siteData2.SiteComponents.WriteToFile(@"C:\Temp\out.json");

        // Assert
        siteData2.SiteComponents.Count().Should().Be(siteData.SiteComponents.Count);
        //siteData2.SiteComponents.Count().Should().Be(1);
        siteData2.SiteComponents.SingleOrDefault(_ => _ is Quad).Should().NotBeNull();
    }
}