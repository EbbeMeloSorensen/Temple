using Craft.Simulation.Bodies;

namespace Temple.ViewModel.DD.Exploration.Bodies;

public class Probe : CircularBody
{
    public Probe(
        int id,
        double radius) : base(id, radius, 1, false, false)
    {
    }
}

