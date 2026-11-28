using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 白骨卫士军官
// Boneguard Lieutenant
// 激励： 获得+1生命值。
// Inspire: Gain +1 Health.
[CardSim("AT_089")]
internal static class Sim_AT_089
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_089e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
