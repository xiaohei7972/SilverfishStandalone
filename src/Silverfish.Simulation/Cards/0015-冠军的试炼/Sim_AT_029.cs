using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 锈水海盗
// Buccaneer
// 每当你装备一把武器，使武器获得+1攻击力。
// Whenever you equip a weapon, give it +1 Attack.
[CardSim("AT_029")]
internal static class Sim_AT_029
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.WeaponEquipped, TriggerRelation.Friendly, [Sim_AT_029e.Rule.ApplyToWeapon()]),
        ],
    };
}
