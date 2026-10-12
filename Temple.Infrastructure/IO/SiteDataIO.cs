using System.Globalization;
using Newtonsoft.Json;
using Craft.Math;
using Craft.Math.IO;
using Temple.Domain.Entities.DD.Common;
using Temple.Domain.Entities.DD.Exploration;
using Temple.Infrastructure.Dialogues;
using Temple.Infrastructure.GameConditions;
using Barrier = Temple.Domain.Entities.DD.Exploration.Barrier;

namespace Temple.Infrastructure.IO;

public static class SiteDataIO
{
    public static string ConvertToJsonRepresentation(
        this IGameCondition gameCondition)
    {
        var json = JsonConvert.SerializeObject(
            gameCondition,
            typeof(IGameCondition),
            Formatting.Indented,
            GetJsonSerializerSettings());

        return json;
    }

    public static IGameCondition ConvertToGameCondition(
        this string json)
    {
        var settings = GetJsonSerializerSettings();

        return JsonConvert.DeserializeObject<IGameCondition>(json, settings);
    }

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
        string[] identifierParts;

        foreach (var geometricObject in geometricObjects)
        {
            switch (geometricObject)
            {
                case LabeledOrientedPoint2D point:

                    identifierParts = point.Identifier.Split("_");

                    switch (identifierParts.First())
                    {
                        case "start":

                            siteData.StartPosition = new Point2D(point.X, -point.Y);

                            var orientation = point.AngleDegrees + 90;

                            if (orientation >= 360)
                            {
                                orientation -= 360;
                            }

                            siteData.StartOrientation = orientation;

                            break;

                        case "door":
                            var doorId = point.Identifier;

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
                                Width = 0.75
                            };

                            point.Labels.Skip(1).ToList().ForEach(label =>
                            {
                                var property = label.Substring(0, label.IndexOf(":")).Trim('\"');

                                var json = label.Substring(label.IndexOf(":") + 1);
                                var condition = json.ConvertToGameCondition();

                                switch (property)
                                {
                                    case "ConditionForPresence":
                                        door.ConditionForPresence = condition;
                                        break;
                                    case "ConditionForAccessibility":
                                        door.ConditionForAccessibility = condition;
                                        break;
                                }
                            });

                            siteData.SiteComponents.Add(door);

                            break;

                        default:
                            throw new InvalidDataException($"Unsupported labeled oriented point: {point.Identifier}");
                    }

                    break;

                case OrientedPoint2D point:
                    throw new NotImplementedException("OrientedPoint2D not supported");

                case Point2D point:
                    throw new NotImplementedException("Point2D not supported");

                case LabeledLineSegment2D labeledLineSegment2D:

                    identifierParts = labeledLineSegment2D.Identifier.Split("_");

                    switch (identifierParts.First())
                    {
                        case "exit":

                            siteData.AddEventTrigger_LeaveSite(
                                new Point2D(labeledLineSegment2D.Point1.X, -labeledLineSegment2D.Point1.Y),
                                new Point2D(labeledLineSegment2D.Point2.X, -labeledLineSegment2D.Point2.Y),
                                labeledLineSegment2D.Identifier);

                            break;
                    }

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
                    typeof(Barrier),
                    typeof(EventTrigger_SiteLocationInfo),
                    typeof(EventTrigger_LeaveSite),
                    typeof(EventTrigger_ScriptedBattle)
                }
            }
        };

        return settings;
    }
}

