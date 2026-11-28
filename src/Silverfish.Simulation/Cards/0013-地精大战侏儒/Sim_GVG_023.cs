using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 地精自动理发装置
// Goblin Auto-Barber
// 战吼：使你的武器获得+1攻击力。
// Battlecry: Give your weapon +1 Attack.
[CardSim("GVG_023")]
internal static class Sim_GVG_023
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ModifyWeaponEffect(AttackDelta: 1, EnchantmentCardId: Sim_GVG_023a.CardId)
        ]
    };
}
