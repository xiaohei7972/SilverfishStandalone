using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 龙鹰骑士
// Dragonhawk Rider
// 激励：在本回合中，获得风怒。
// Inspire: Gain Windfury this turn.
[CardSim("AT_083")]
internal static class Sim_AT_083
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(TriggerTiming.AfterHeroPowerUsed, TriggerRelation.Friendly, [Sim_AT_083e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
