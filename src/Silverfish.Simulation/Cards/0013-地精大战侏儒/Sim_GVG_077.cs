using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 心能魔像
// Anima Golem
// 在每个回合结束时，如果本随从是你唯一的随从，则消灭 本随从。
// At the end of each turn, destroy this minion if it's your only one.
[CardSim("GVG_077")]
internal static class Sim_GVG_077
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnEnded,
                TriggerRelation.Any,
                [
                    new ConditionalEffect(
                        new ZoneCountCondition(
                            ConditionPlayer.Friendly,
                            GameZone.Play,
                            NumberComparison.Equal,
                            1,
                            EntityKind.Minion),
                        [new DestroyMinionEffect(EffectTarget.Source)])
                ])
        ]
    };
}
