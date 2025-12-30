using Craft.Simulation.Bodies;

namespace Temple.ViewModel.DD.Exploration.Bodies;

public class Player : CircularBody
{
    public Player(
        int id,
        double radius) : base(id, radius, 1, false)
    {
    }
}

