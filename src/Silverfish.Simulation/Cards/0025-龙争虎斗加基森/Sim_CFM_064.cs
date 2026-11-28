using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑金大亨
// Blubber Baron
// 每当你召唤一个具有战吼的随从时，便使这张牌（在你手牌中时）获得+1/+1。
// Whenever you summon a Battlecry minion while this_is in your hand, gain_+1/+1.
[CardSim("CFM_064")]
internal static class Sim_CFM_064
{
    internal static CardRules Rules { get; } = new()
    {
        InHandTriggers =
        [
            new(TriggerTiming.AfterMinionSummoned, TriggerRelation.Friendly,
                [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 1, 1))
                    { EnchantmentCardId = Sim_CFM_064e.CardId }])
            {
                CardFilter = new CardFilter { HasBattlecry = true },
            },
        ],
    };
}
