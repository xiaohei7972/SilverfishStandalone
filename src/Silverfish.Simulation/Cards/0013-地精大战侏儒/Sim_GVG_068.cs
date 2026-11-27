using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 石腭穴居人壮汉
// Burly Rockjaw Trogg
// 每当你的对手施放一个法术，获得 +2攻击力。
// Whenever your opponent casts a spell, gain +2 Attack.
[CardSim("GVG_068")]
internal static class Sim_GVG_068
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.BeforeSpellResolved,
                TriggerRelation.Opponent,
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 2))
                    {
                        EnchantmentCardId = Sim_GVG_068a.CardId
                    }
                ])
        ]
    };
}
