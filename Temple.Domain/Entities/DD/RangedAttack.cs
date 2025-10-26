namespace Temple.Domain.Entities.DD;

public class RangedAttack : Attack
{
    public double Range { get; set; }

    public RangedAttack(
        string name,
        int maxDamage,
        double range) : base(name, maxDamage)
    {
        Range = range;
    }
}