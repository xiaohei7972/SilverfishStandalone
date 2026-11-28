using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 光明邪使菲奥拉
// Fjola Lightbane
// 每当你以本随从为目标施放一个法术时，获得圣盾。
// Whenever you target this minion with a spell, gain Divine Shield.
[CardSim("AT_129")]
internal static class Sim_AT_129
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.TargetedBySpell, TriggerRelation.Friendly, [new EnchantStatsEffect(StatTarget.Source) { GrantedTag = GAME_TAG.DIVINE_SHIELD }]) { EntityMatch = TriggerEntityMatch.Self },
        ],
    };
}
