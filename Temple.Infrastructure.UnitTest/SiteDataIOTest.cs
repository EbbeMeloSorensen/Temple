using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.UnitTest;

public class SiteDataIOTest
{
    [Fact]
    public void WriteSiteDataToFile()
    {
        // Arrange
        var siteData = new SiteData();

        // Act
        siteData.WriteToFile(@"C:\Temp\test_site_data.json");
    }
}