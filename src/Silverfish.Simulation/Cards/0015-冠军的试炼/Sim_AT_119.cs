using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 克瓦迪尔劫掠者
// Kvaldir Raider
// 激励：获得+2/+2。
// Inspire: Gain +2/+2.
[CardSim("AT_119")]
internal static class Sim_AT_119
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_119e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
