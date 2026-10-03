using System.Globalization;
using Craft.Math;
using Newtonsoft.Json;
using Craft.Math.IO;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.GameConditions;

namespace Temple.Infrastructure.IO;

public static class SiteDataIO
{
    public static void WriteSiteDataToFile(
        this SiteData siteData,
        string fileName)
    {
        var json = JsonConvert.SerializeObject(
            siteData,
            Formatting.Indented,
            GetJsonSerializerSettings());

        using var streamWriter = new StreamWriter(fileName);

        streamWriter.WriteLine(json);
    }

    public static SiteData ReadSiteDataFromFile(
        string fileName)
    {
        using var streamReader = new StreamReader(fileName);
        var json = streamReader.ReadToEnd();
        var settings = GetJsonSerializerSettings();

        return JsonConvert.DeserializeObject<SiteData>(json, settings);
    }

    public static SiteData ImportSiteDataFromFile(
        string fileName)
    {
        using var r = new StreamReader(fileName);
        var jsonData = r.ReadToEnd();
        var geometricObjects = GeometryFile.Deserialize(jsonData);

        var siteData = new SiteData();

        foreach (var geometricObject in geometricObjects)
        {
            switch (geometricObject)
            {
                case LabeledOrientedPoint2D point:

                    var doorId = point.Text;

                    var doorOrientation = point.AngleDegrees + 90;

                    if (doorOrientation >= 360)
                    {
                        doorOrientation -= 360;
                    }

                    var door = new Door
                    {
                        Id = doorId,
                        Position = new Vector3D(point.X, -point.Y, 0),
                        Orientation = doorOrientation,
                        Width = 0.75,
                        Condition = null,
                        ConditionForAccessibility = null
                    };

                    siteData.SiteComponents.Add(door);

                    break;
                case OrientedPoint2D point:
                    throw new NotImplementedException("OrientedPoint2D not supported");
                case Point2D point:
                    throw new NotImplementedException("Point2D not supported");
                    break;
                case LineSegment2D lineSegment2D:
                    siteData.AddWall(new List<Point2D>
                    {
                        new Point2D(lineSegment2D.Point1.X, -lineSegment2D.Point1.Y),
                        new Point2D(lineSegment2D.Point2.X, -lineSegment2D.Point2.Y)
                    });

                    break;
                default:
                    throw new InvalidDataException("Unsupported geometry type.");
            }
        }

        return siteData;
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var settings = new JsonSerializerSettings
        {
            Culture = CultureInfo.InvariantCulture,
            ContractResolver = new SiteComponentResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            SerializationBinder = new KnownTypesBinder
            {
                KnownTypes = new[]
                {
                    typeof(KnowledgeGainedCondition),
                    typeof(FactEstablishedCondition),
                    typeof(QuestStatusCondition),
                    typeof(BattleWonCondition),
                    typeof(AndGameCondition),
                    typeof(OrGameCondition),
                    typeof(NotGameCondition),
                    typeof(Quad),
                    typeof(Cylinder),
                    typeof(Sphere),
                    typeof(NPC),
                    typeof(Door),
                    typeof(Domain.Entities.DD.Exploration.Barrier),
                    typeof(EventTrigger_SiteLocationInfo),
                    typeof(EventTrigger_LeaveSite),
                    typeof(EventTrigger_ScriptedBattle)
                }
            }
        };

        return settings;
    }
}

