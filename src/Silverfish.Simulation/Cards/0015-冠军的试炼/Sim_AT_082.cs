using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 低阶侍从
// Lowly Squire
// 激励： 获得+1攻击力。
// Inspire: Gain +1 Attack.
[CardSim("AT_082")]
internal static class Sim_AT_082
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_082e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
