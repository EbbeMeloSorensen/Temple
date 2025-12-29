using Craft.Math;
using Temple.Domain.Entities.DD.Exploration;

namespace Temple.ViewModel.DD.Exploration;

public static class SiteDataFactory
{
    public static SiteData GenerateSiteData(
        string siteId)
    {
        switch (siteId)
        {
            case "Mine":
            {
                var siteData = new SiteData();

                var siteExtent = 20.0;

                siteData.AddQuad(
                    new Point3D(siteExtent, siteExtent, 0),
                    new Point3D(-siteExtent, siteExtent, 0),
                    new Point3D(-siteExtent, -siteExtent, 0),
                    new Point3D(siteExtent, -siteExtent, 0));

                siteData.AddQuad(
                    new Point3D(siteExtent, siteExtent, 1),
                    new Point3D(siteExtent, -siteExtent, 1),
                    new Point3D(-siteExtent, -siteExtent, 1),
                    new Point3D(-siteExtent, siteExtent, 1));

                siteData.AddWall(new List<Point2D>
                {
                    new (1, 0),
                    new (1, 2),
                    new (2, 2),
                    new (2, 1),
                    new (4, 1),
                    new (4, 2),
                    new (5, 2),
                    new (5, 1),
                    new (7, 1),
                    new (7, 6),
                    new (8, 6),
                    new (8, 9),
                    new (5, 9),
                    new (5, 7),
                    new (3, 7),
                    new (3, 9),
                    new (-2, 9),
                    new (-2, 5),
                    new (0, 5),
                    new (0, 3),
                    new (-1, 3),
                    new (-1, 4),
                    new (-3, 4),
                    new (-3, 1),
                    new (-1, 1),
                    new (-1, 2),
                    new (0, 2),
                    new (0, 0)
                });

                siteData.AddWall(new List<Point2D>
                {
                    new (1, 3),
                    new (1, 5),
                    new (3, 5),
                    new (3, 6),
                    new (6, 6),
                    new (6, 5),
                    new (5, 5),
                    new (5, 3),
                    new (4, 3),
                    new (4, 4),
                    new (2, 4),
                    new (2, 3),
                    new (1, 3),
                });

                siteData.AddEventTrigger_LeaveSite(
                    new Point2D(0, 0),
                    new Point2D(1, 0),
                    "Exit_To_Wilderness");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(-1, 3),
                    new Point2D(-1, 2),
                    "Dungeon 1, Room A, Goblin");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(2, 3),
                    new Point2D(2, 2),
                    "Dungeon 1, Room B, Goblin",
                    "West");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(4, 3),
                    new Point2D(4, 2),
                    "Dungeon 1, Room B, Goblin",
                    "East");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(1, 5),
                    new Point2D(0, 5),
                    "Final Battle",
                    "South");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(3, 7),
                    new Point2D(3, 6),
                    "Final Battle",
                    "East");

                return siteData;
            }
            case "Village":
            {
                var siteData = new SiteData();

                siteData.AddWall(new List<Point2D>
                {
                    new (8, 7),
                    new (8, 4),
                    new (4, 4),
                    new (4, 11),
                    new (8, 11),
                    new (8, 8),
                    new (8, 11),
                    new (4, 11),
                    new (4, 4),
                    new (8, 4),
                    new (8, 7)
                });

                siteData.AddWall(new List<Point2D>
                {
                    new (15, 8),
                    new (15, 12),
                    new (3, 12),
                    new (3, 3),
                    new (15, 3),
                    new (15, 7),
                });

                siteData.AddWall(new List<Point2D>
                {
                    new (12, 6),
                    new (13, 6),
                    new (13, 4),
                    new (10, 4),
                    new (10, 6),
                    new (11, 6),
                    new (10, 6),
                    new (10, 4),
                    new (13, 4),
                    new (13, 6),
                    new (12, 6)
                });

                siteData.AddWall(new List<Point2D>
                {
                    new (11, 9),
                    new (10, 9),
                    new (10, 11),
                    new (13, 11),
                    new (13, 9),
                    new (12, 9),
                    new (13, 9),
                    new (13, 11),
                    new (10, 11),
                    new (10, 9),
                    new (11, 9),
                });

                siteData.AddEventTrigger_LeaveSite(
                    new Point2D(15, 8),
                    new Point2D(15, 7),
                    "Exit_To_Wilderness");

                return siteData;
            }
            default:
            {
                throw new InvalidOperationException($"Unknown site id: {siteId}");
            }
        }
    }
}

