using Temple.Domain.Entities.DD.Battle;

namespace Temple.ViewModel.DD.BusinessLogic.Complex
{
    public static class BoardItemExtensions
    {
        public static int IndexOfOccupiedSquare(
            this BoardItem boardItem,
            int columns)
        {
            return boardItem.PositionY * columns + boardItem.PositionX;
        }
    }
}
