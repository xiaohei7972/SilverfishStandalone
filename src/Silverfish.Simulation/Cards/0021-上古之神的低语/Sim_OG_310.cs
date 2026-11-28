using Silverfish.Domain.Client;

namespace Silverfish.Simulation.Cards;

// 夜色镇执法官
// Steward of Darkshire
// 每当你召唤一个生命值为1的随从，便使其获得圣盾。
// Whenever you summon a 1-Health minion, give it Divine Shield.
[CardSim("OG_310")]
internal static class Sim_OG_310
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.WhenMinionSummoned, TriggerRelation.Friendly, [new EnchantStatsEffect(StatTarget.TriggerSubject) { GrantedTag = GAME_TAG.DIVINE_SHIELD }]) { Condition = new CharacterCondition(ConditionCharacter.TriggerSubject, CharacterMetric.Health, NumberComparison.Equal, 1) },
        ],
    };
}
