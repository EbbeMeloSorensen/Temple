using Craft.Math;
using Temple.Application.Interfaces;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.Exploration;

public class SiteDataFactory : ISiteDataFactory
{
    public SiteData GenerateSiteData(
        string siteId)
    {
        if (siteId == "maze")
        {
            var siteData = new SiteData();

            siteData.AddEventTrigger_LeaveSite(
                new Point2D(1, -1),
                new Point2D(2, -1),
                "Exit_Wilderness");

            // Det her er ikke så tungt
            var rows = 4;
            var cols = 4;

            // 10 x 10 -> Det er tungt i scenen, hvor der skal checkes for kollision med 10 * 10 * 4 = 400 liniestykker
            // Det er ikke så tungt for 3D-modellerne, hvor der er 10 * 10 * 4 * 2 = 800 trekanter
            // (det kan man se ved at udkommentere den del, der laver quads til liniestykker i 2D-scenen)
            //var rows = 10;
            //var cols = 10;

            // 100 x 100
            // Det her er tungt i selve 3D-modellen, dvs også selv om man har udkommenteret den del, der laver quads til liniestykker i 2D-scenen
            //var rows = 100;
            //var cols = 100;

            var x0 = 0.5;
            var y0 = 0.5;

            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    var x = x0 + r * 2.0;
                    var y = y0 + c * 2.0;

                    AddDummyWallToSite(siteData, x, y);
                }
            }

            return siteData;
        }

        return new SiteData
        {
            SiteComponents = SiteDataIO.ReadSiteComponentListFromFile(
                $"DD//Assets//SiteData//{siteId}.json").ToList()
        };
    }

    private void AddDummyWallToSite(
        SiteData siteData,
        double x,
        double y)
    {
        siteData.AddWall(new List<Point2D>
        {
            new Point2D(x - 0.5, y + 0.5),
            new Point2D(x + 0.5, y + 0.5),
            new Point2D(x + 0.5, y - 0.5),
            new Point2D(x - 0.5, y - 0.5),
            new Point2D(x - 0.5, y + 0.5),
        });
    }
}

