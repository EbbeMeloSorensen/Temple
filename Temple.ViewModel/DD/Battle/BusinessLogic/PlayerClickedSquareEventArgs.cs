namespace Temple.ViewModel.DD.Battle.BusinessLogic
{
    public class PlayerClickedSquareEventArgs : EventArgs
    {
        public readonly int SquareIndex;

        public PlayerClickedSquareEventArgs(int squareIndex)
        {
            SquareIndex = squareIndex;
        }
    }
}
