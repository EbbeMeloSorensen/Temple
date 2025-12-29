namespace Temple.Domain.Entities.DD.Battle;

public class MeleeAttack : Attack
{
    public MeleeAttack(
        string name,
        int maxDamage) : base(name, maxDamage)
    {
    }
}