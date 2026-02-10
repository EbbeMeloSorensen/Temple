namespace Temple.Domain.Entities.WMDR
{
    public class Point : GeospatialLocation
    {
        public string CoordinateSystem { get; set; }
        public double Coordinate1 { get; set; }
        public double Coordinate2 { get; set; }

        public override string ToString()
        {
            var result = $"({Coordinate1}, {Coordinate2}), From: {From.ToShortDateString()}";

            if (To < DateTime.MaxValue)
            {
                result += $" To: {To.ToShortDateString()}";
            }

            return result;
        }
    }
}