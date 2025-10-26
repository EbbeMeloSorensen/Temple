namespace Temple.ViewModel.DD.BusinessLogic
{
    public static class IntExtensions
    {
        public static int ConvertToXCoordinate(
            this int squareIndex,
            int columns)
        {
            return squareIndex % columns;
        }

        public static int ConvertToYCoordinate(
            this int squareIndex,
            int columns)
        {
            return squareIndex / columns;
        }
    }
}
