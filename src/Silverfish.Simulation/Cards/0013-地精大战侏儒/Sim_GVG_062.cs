using Silverfish.Domain;
using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 钴制卫士
// Cobalt Guardian
// 每当你召唤一个机械，便获得圣盾。
// Whenever you summon a Mech, gain Divine Shield.
[CardSim("GVG_062")]
internal static class Sim_GVG_062
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.WhenMinionSummoned,
                TriggerRelation.Friendly,
                [new EnchantStatsEffect(StatTarget.Source, null) { GrantedTag = GAME_TAG.DIVINE_SHIELD }])
            {
                CardFilter = new CardFilter { MinionType = MinionType.Mech }
            }
        ]
    };
}
