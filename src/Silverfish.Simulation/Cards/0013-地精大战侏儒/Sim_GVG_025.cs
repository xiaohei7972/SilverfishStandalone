using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 独眼欺诈者
// One-eyed Cheat
// 每当你召唤一个海盗，便获得潜行。
// Whenever you summon a Pirate, gain Stealth.
[CardSim("GVG_025")]
internal static class Sim_GVG_025
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.WhenMinionSummoned,
                TriggerRelation.Friendly,
                [new EnchantStatsEffect(StatTarget.Source, null) { GrantedTag = GAME_TAG.STEALTH }])
            {
                CardFilter = new CardFilter { MinionType = MinionType.Pirate }
            }
        ]
    };
}
