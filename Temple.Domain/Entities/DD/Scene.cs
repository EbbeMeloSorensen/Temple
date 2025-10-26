namespace Temple.Domain.Entities.DD
{
    public class Scene
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public int Rows { get; set; }
        public int Columns { get; set; }

        public List<Obstacle> Obstacles { get; }
        public List<Creature> Creatures { get; }

        public Scene()
        {
            Obstacles = new List<Obstacle>();
            Creatures = new List<Creature>();
        }

        public Scene(
            string name,
            int rows,
            int columns)
        {
            Name = name;
            Rows = rows;
            Columns = columns;

            Obstacles = new List<Obstacle>();
            Creatures = new List<Creature>();
        }

        public void AddObstacle(
            Obstacle obstacle)
        {
            Obstacles.Add(obstacle);
        }

        public void AddCreature(
            Creature creature,
            int positionX,
            int positionY)
        {
            creature.PositionX = positionX;
            creature.PositionY = positionY;
            Creatures.Add(creature);
        }
    }
}
