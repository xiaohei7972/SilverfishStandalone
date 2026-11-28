using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暗影之握
// Embrace the Shadow
// 在本回合中，你的治疗效果转而造成等量的伤害。
// This turn, your healing effects deal damage instead.
[CardSim("OG_104")]
internal static class Sim_OG_104
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new HealingDealsDamageThisTurnEffect(Sim_OG_104e.CardId),
        ],
    };
}
