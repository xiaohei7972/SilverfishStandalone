using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 钢铁武道家
// Iron Sensei
// 在你的回合结束时，使另一个友方机械获得+2/+2。
// At the end of your turn, give another friendly Mech +2/+2.
[CardSim("GVG_027")]
internal static class Sim_GVG_027
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Friendly,
                [
                    new EnchantStatsEffect(StatTarget.RandomOtherFriendlyMinion, new(StatModifierMode.Add, 2, 2))
                    {
                        CardFilter = new CardFilter { MinionType = MinionType.Mech },
                        EnchantmentCardId = Sim_GVG_027e.CardId
                    }
                ])
        ]
    };
}
