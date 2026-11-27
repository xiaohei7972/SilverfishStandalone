using Silverfish.Domain;

namespace Silverfish.Simulation.Cards;

// 微型战斗机甲
// Micro Machine
// 在每个回合开始时，获得+1攻击力。
// At the start of each turn, gain +1 Attack.
[CardSim("GVG_103")]
internal static class Sim_GVG_103
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnStarted,
                TriggerRelation.Any,
                [new EnchantStatsEffect(StatTarget.Source, new(StatModifierMode.Add, 1))])
        ]
    };
}
