using Craft.DataStructures.Geometry;

namespace Temple.ViewModel.DD.Exploration
{
    public static class Helpers
    {
        public static BoundingBox ComputeBoundingBox(
            this Craft.Math.Point2D point)
        {
            return new BoundingBox(
                point.X,
                point.X,
                point.Y,
                point.Y);
        }

        public static BoundingBox ComputeBoundingBox(
            this Craft.Math.LineSegment2D lineSegment)
        {
            var minX = Math.Min(lineSegment.Point1.X, lineSegment.Point2.X);
            var maxX = Math.Max(lineSegment.Point1.X, lineSegment.Point2.X);
            var minY = Math.Min(lineSegment.Point1.Y, lineSegment.Point2.Y);
            var maxY = Math.Max(lineSegment.Point1.Y, lineSegment.Point2.Y);

            return new BoundingBox(minX, maxX, minY, maxY);
        }

        public static BoundingBox ComputeBoundingBox(
            this Craft.Math.Circle2D circle)
        {
            return new BoundingBox(
                circle.Center.X - circle.Radius,
                circle.Center.X + circle.Radius,
                circle.Center.Y - circle.Radius,
                circle.Center.Y + circle.Radius);
        }
    }
}
