using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血吼
// Gorehowl
// 攻击随从不会消耗耐久度，改为降低1点攻击力。
// Attacking a minion costs 1 Attack instead of 1 Durability.
[CardSim("EX1_411")]
internal static class Sim_EX1_411
{
    internal static CardRules Rules { get; } = new()
    {
        MinionAttackRule = new(1, Sim_EX1_411e2.CardId)
    };
}
