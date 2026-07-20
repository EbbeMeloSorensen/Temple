using FluentAssertions;
using Craft.Math;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Domain.Entities.DD.Quests;
using Temple.Infrastructure.GameConditions;
using Temple.Infrastructure.IO;

namespace Temple.Infrastructure.UnitTest;

public class SiteDataIOTest
{
    [Fact]
    public void WriteSiteDataToFileThenReadItAndWriteItAgain()
    {
        // Arrange
        var siteData = new SiteData
        {
            StartPosition = new Point2D(3, 4),
            StartOrientation = 90.0
        };

        var siteExtent = 20.0;

        siteData.AddEventTrigger_ScriptedBattle(
            new Point2D(12, 9),
            new Point2D(11, 9),
            "rats_in_warehouse",
            null,
            null,
            new QuestStatusCondition
            {
                QuestId = "rat_infestation",
                RequiredStatus = new QuestStatus
                {
                    QuestState = QuestState.Active,
                    AreCompletionCriteriaSatisfied = false
                }
            });

        siteData.AddCharacter(
            "human male",
            "lortimer",
            new Point2D(12.5, 8.3));

        siteData.AddSphere(new Point2D(10.5, 8.5), 0.1, 0.4);

        siteData.AddCylinder(new Point2D(10.5, 8.5), 0.1, 0.4);

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

        siteData.AddDoor(
            "door1",
            new Point2D(0, 0),
            90,
            null,
            new FactEstablishedCondition
            {
                FactId = "got_key_to_cellar_door_from_ethon"
            });

        siteData.AddEventTrigger_LeaveSite(
            new Point2D(0, 0),
            new Point2D(1, 0),
            "Exit_Wilderness");

        siteData.AddEventTrigger_ScriptedBattle(
            new Point2D(-1, 3),
            new Point2D(-1, 2),
            "Dungeon 1, Room A, Goblin");

        var fileName = @"C:\Temp\test_site_data.json";

        // Act (Write)
        siteData.WriteSiteDataToFile(fileName);

        // Act (Read)
        var siteData2 = SiteDataIO.ReadSiteDataFromFile(fileName);

        // Act (Write the one that was read)
        siteData2.WriteSiteDataToFile(@"C:\Temp\out.json");

        // Assert
        siteData2.SiteComponents.Count().Should().Be(siteData.SiteComponents.Count);
        siteData2.SiteComponents.SingleOrDefault(_ => _ is Quad).Should().NotBeNull();
    }

    [Fact]
    public void ReadSiteIdsFromFile()
    {
        // Arrange

        // Act

        // Assert
    }
}