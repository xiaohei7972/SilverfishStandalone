using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 格鲁尔
// Gruul
// 在每个回合结束时，获得+1/+1。
// At the end of each turn, gain +1/+1 .
[CardSim("NEW1_038")]
internal static class Sim_NEW1_038
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers = [new(TriggerTiming.TurnEnded, TriggerRelation.Any, [Sim_NEW1_038o.Rule.ApplyTo(StatTarget.Source)])]
    };
}
