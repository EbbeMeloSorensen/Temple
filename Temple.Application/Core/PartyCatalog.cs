using Temple.Domain.Entities.DD.Battle;

namespace Temple.Application.Core;

// Existing starter definitions, now with stable identities for saved party members.
internal static class PartyCatalog
{
    public static Dictionary<Guid, CreatureType> CreateTypes()
    {
        // Attacks
        var longSword = new MeleeAttack(name: "Longsword", maxDamage: 10);
        var bowAndArrow = new RangedAttack(name: "Bow & Arrow", maxDamage: 4, range: 5);

        // Creature types
        var knight = new CreatureType("Knight",
            maxHitPoints: 8,
            armorClass: 3,
            thaco: 5,
            initiativeModifier: 0,
            movement: 4,
            attacks: new List<Attack>
            {
                longSword,
                longSword,
                longSword
            });

        var archer = new CreatureType(
            name: "Archer",
            maxHitPoints: 12,
            armorClass: 6,
            thaco: 14,
            initiativeModifier: 10,
            movement: 6,
            attacks: new List<Attack>
            {
                bowAndArrow,
                bowAndArrow
            });

        knight.ID = Guid.Parse("f3827714-2366-42ca-b053-c1f229e2d001");
        archer.ID = Guid.Parse("f3827714-2366-42ca-b053-c1f229e2d002");
        return new Dictionary<Guid, CreatureType>
        {
            [knight.ID] = knight,
            [archer.ID] = archer
        };
    }
}
