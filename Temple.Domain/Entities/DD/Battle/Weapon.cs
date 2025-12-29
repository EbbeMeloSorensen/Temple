namespace Temple.Domain.Entities.DD.Battle
{
    public class Weapon : BoardItem
    {
        public Weapon(
            int positionX,
            int positionY)
        {
            PositionX = positionX;
            PositionY = positionY;
        }
    }
}
