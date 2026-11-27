using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 牛头人战士
// Tauren Warrior
// 嘲讽 受伤时拥有+3攻 击力。
// Taunt Has +3 Attack while damaged.
[CardSim("EX1_390")]
internal static class Sim_EX1_390
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_390e.Rule.GrantTo(AuraRecipients.Self, AuraCondition.WhileDamaged)]
    };
}
