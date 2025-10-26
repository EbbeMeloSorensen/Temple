using Temple.Domain.Entities.DD;

namespace Temple.ViewModel.DD
{
    public class ObstacleViewModel : BoardItemViewModel
    {
        public ObstacleViewModel(
            Obstacle obstacle,
            double left,
            double top,
            double diameter) : base(left, top, diameter)
        {
            switch (obstacle.ObstacleType)
            {
                case ObstacleType.Wall:
                    ImagePath = "DD/Images/Wall.jpg";
                    break;
                case ObstacleType.Water:
                    ImagePath = "DD/Images/Water.PNG";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(obstacle.ObstacleType), obstacle.ObstacleType, null);
            }
        }
    }
}
