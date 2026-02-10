using Craft.Math;
using Temple.Application.Interfaces.Readers;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Entities.DD.Quests;

namespace Temple.ViewModel.DD.Exploration;

public static class SiteDataFactory
{
    public static SiteData GenerateSiteData(
        string siteId,
        IQuestStatusReader questStatusReadModel)
    {
        var siteData = new SiteData();

        switch (siteId)
        {
            case "Mine":
            {
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
                    "Exit_Wilderness");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(-1, 3),
                    new Point2D(-1, 2),
                    "Dungeon 1, Room A, Goblin");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(2, 3),
                    new Point2D(2, 2),
                    "Dungeon 1, Room B, Goblin",
                    null,
                    "West");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(4, 3),
                    new Point2D(4, 2),
                    "Dungeon 1, Room B, Goblin",
                    null,
                    "East");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(1, 5),
                    new Point2D(0, 5),
                    "Final Battle",
                    null,
                    "South");

                siteData.AddEventTrigger_ScriptedBattle(
                    new Point2D(3, 7),
                    new Point2D(3, 6),
                    "Final Battle",
                    null,
                    "East");

                break;
            }
            case "Village":
            {
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

                siteData.AddCylinder(new Point2D(10.5, 8.5), 0.1, 0.4);

                siteData.AddSphere(new Point2D(10.5, 8.5), 0.1, 0.4);

                siteData.AddCharacter("human male", "guard", new Point2D(8.5, 6.5));
                siteData.AddCharacter("human male", "captain", new Point2D(8.5, 8.5));
                siteData.AddCharacter("human female", "alyth", new Point2D(12.5, 7.5));
                siteData.AddCharacter("human male", "ethon", new Point2D(12.5, 7.9));

                    siteData.AddEventTrigger_LeaveSite(
                    new Point2D(15, 8),
                    new Point2D(15, 7),
                    "Exit_Wilderness");

                if (questStatusReadModel.GetQuestStatus("rat_infestation").QuestState == QuestState.Active)
                {
                    siteData.AddEventTrigger_ScriptedBattle(
                        new Point2D(12, 9),
                        new Point2D(11, 9),
                        "rats_in_warehouse");
                }

                break;
            }
            case "Graveyard":
            {
                siteData.AddWall(new List<Point2D>
                {
                    new (15, 8),
                    new (15, 12),
                    new (3, 12),
                    new (3, 3),
                    new (15, 3),
                    new (15, 7),
                });

                siteData.AddEventTrigger_LeaveSite(
                    new Point2D(15, 8),
                    new Point2D(15, 7),
                    "Exit_Wilderness");

                if (questStatusReadModel.GetQuestStatus("skeleton_trouble").QuestState == QuestState.Active)
                {
                    siteData.AddEventTrigger_ScriptedBattle(
                        new Point2D(12, 9),
                        new Point2D(11, 9),
                        "skeletons_in_graveyard");
                }

                break;
            }
            default:
            {
                throw new InvalidOperationException($"Unknown site id: {siteId}");
            }
        }

        return siteData;
    }
}

