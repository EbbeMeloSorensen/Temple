using Craft.Math;
using Temple.Domain.Entities.C2IEDM.ObjectItems;

namespace Temple.Domain.Entities.DD.Exploration;

public class SiteData
{
    private readonly List<SiteComponent> _siteComponents = new();

    public IReadOnlyList<SiteComponent> SiteComponents => _siteComponents;

    public void AddQuad(
        Point3D point1,
        Point3D point2,
        Point3D point3,
        Point3D point4)
    {
        _siteComponents.Add(new Quad("quad")
        {
            Point1 = new Vector3D(point1.Y, point1.Z, point1.X),
            Point2 = new Vector3D(point2.Y, point2.Z, point2.X),
            Point3 = new Vector3D(point3.Y, point3.Z, point3.X),
            Point4 = new Vector3D(point4.Y, point4.Z, point4.X)
        });
    }

    public void AddSphere(
        Point2D position,
        double radius,
        double height = 0)
    {
        _siteComponents.Add(new Sphere("sphere")
        {
            Position = new Vector3D(position.Y, height, position.X),
            Radius = radius
        });
    }

    public void AddCylinder(
        Point2D position,
        double radius,
        double length,
        double height = 0)
    {
        _siteComponents.Add(new Cylinder("cylinder")
        {
            Position = new Vector3D(position.Y, height, position.X),
            Radius = radius,
            Length = length
        });
    }

    public void AddExclamationMark(
        Point2D position,
        double height = 0)
    {
        _siteComponents.Add(new ExclamationMark("exclamation mark")
        {
            Position = new Vector3D(position.Y, height, position.X)
        });
    }

    public void AddWall(
        IEnumerable<Point2D> wallPoints)
    {
        _siteComponents.Add(new Barrier("wall")
        {
            BarrierPoints = wallPoints.Select(_ => new Vector3D(_.Y, 0, _.X)).ToList()
        });
    }

    public void AddCharacter(
        string modelId,
        string name,
        Point2D position,
        double orientation = 0,
        double height = 0,
        string? questId = null)
    {
        _siteComponents.Add(new NPC(modelId)
        {
            Name = name,
            QuestId = questId,
            Position = new Vector3D(position.Y, height, position.X),
            Orientation = orientation
        });

        if (questId != null)
        {
            _siteComponents.Add(new ExclamationMark("exclamation mark")
            {
                Position = new Vector3D(
                    position.Y,
                    height + 0.6,
                    position.X)
            });
        }
    }

    public void AddEventTrigger_LeaveSite(
        Point2D point1,
        Point2D point2,
        string eventId)
    {
        _siteComponents.Add(new EventTrigger_LeaveSite("event trigger")
        {
            Point1 = point1,
            Point2 = point2,
            EventID = eventId
        });
    }

    public void AddEventTrigger_ScriptedBattle(
        Point2D point1,
        Point2D point2,
        string eventId,
        int? questId = null,
        string? entranceId = null)
    {
        _siteComponents.Add(new EventTrigger_ScriptedBattle("event trigger")
        {
            Point1 = point1,
            Point2 = point2,
            EventID = eventId,
            QuestID = questId,
            EntranceID = entranceId
        });
    }
}
