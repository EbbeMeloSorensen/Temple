using Craft.Simulation.Bodies;

namespace Temple.ViewModel.DD.Exploration.Bodies;

public class NPC : CircularBody
{
    public NPC(
        int id,
        double radius,
        string tag) : base(id, radius, 1, false, false, tag)
    {
    }
}

