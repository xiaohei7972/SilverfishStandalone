using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 升级
// Upgrade!
// 如果你装备着武器，使其获得+1/+1。否则装备一把1/3的武器。
// If you have a weapon, give it +1/+1. Otherwise equip a 1/3 weapon.
[CardSim("EX1_409")]
internal static class Sim_EX1_409
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new WeaponEquippedCondition(), [Sim_EX1_409e.Rule.ApplyToWeapon()])
            {
                Otherwise = [new EquipWeaponEffect("EX1_409t")]
            }
        ]
    };
}
