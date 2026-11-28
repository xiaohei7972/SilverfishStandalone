using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 恶毒铁匠
// Spiteful Smith
// 本随从受伤时，你的武器拥有+2攻击力。
// Your weapon has +2 Attack while this is damaged.
[CardSim("CS2_221")]
internal static class Sim_CS2_221
{
    internal static CardRules Rules { get; } = new()
    {
        WeaponAuras = [Sim_CS2_221e.Rule.GrantToWeapon(AuraCondition.WhileDamaged)]
    };
}
