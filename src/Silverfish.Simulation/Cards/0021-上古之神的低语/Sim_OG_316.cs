using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 传令官沃拉兹
// Herald Volazj
// 战吼：召唤所有其他友方随从的复制，他们均为1/1。
// Battlecry: Summon a 1/1 copy of each of your other minions.
[CardSim("OG_316")]
internal static class Sim_OG_316
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new SummonCopyEffect { AllOtherFriendlyMinions = true, StatsOverride = Sim_OG_316k.Rule.Modifier, EnchantmentCardId = Sim_OG_316k.Rule.CardId },
        ],
    };
}
