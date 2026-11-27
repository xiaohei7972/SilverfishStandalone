using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 碎石穴居人
// Stonesplinter Trogg
// 每当你的对手施放一个法术，便获得+1攻击力。
// Whenever your opponent casts a spell, gain +1 Attack.
[CardSim("GVG_067")]
internal static class Sim_GVG_067
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.BeforeSpellResolved,
                TriggerRelation.Opponent,
                [
                    new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 1))
                    {
                        EnchantmentCardId = Sim_GVG_067a.CardId
                    }
                ])
        ]
    };
}
