using Craft.Math;

namespace Temple.Domain.Entities.DD.Exploration;

public class SiteData
{
    private List<SiteComponent> _siteComponents;

    public IReadOnlyList<SiteComponent> SiteComponents => _siteComponents;

    public SiteData()
    {
        _siteComponents = new List<SiteComponent>();
    }

    public void AddQuad(
        Point3D point1,
        Point3D point2,
        Point3D point3,
        Point3D point4)
    {
        var pt1 = new Vector3D(point1.Y, point1.Z, point1.X);

        _siteComponents.Add(new Quad("quad")
        {
            Point1 = new Vector3D(point1.Y, point1.Z, point1.X),
            Point2 = new Vector3D(point2.Y, point2.Z, point2.X),
            Point3 = new Vector3D(point3.Y, point3.Z, point3.X),
            Point4 = new Vector3D(point4.Y, point4.Z, point4.X)
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
        string? entranceId = null)
    {
        _siteComponents.Add(new EventTrigger_ScriptedBattle("event trigger")
        {
            Point1 = point1,
            Point2 = point2,
            EventID = eventId,
            EntranceID = entranceId
        });
    }
}
