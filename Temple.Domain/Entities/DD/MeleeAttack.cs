namespace Temple.Domain.Entities.DD;

public class MeleeAttack : Attack
{
    public MeleeAttack(
        string name,
        int maxDamage) : base(name, maxDamage)
    {
    }
}