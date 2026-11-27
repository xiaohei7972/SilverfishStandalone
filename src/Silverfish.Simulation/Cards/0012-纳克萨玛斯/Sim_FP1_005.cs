using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 纳克萨玛斯之影
// Shade of Naxxramas
// 潜行。在你的回合开始时，获得+1/+1。
// Stealth. At the start of your turn, gain +1/+1.
[CardSim("FP1_005")]
internal static class Sim_FP1_005
{
    internal static CardRules Rules { get; } = new()
    {
        Triggers =
        [
            new(
                TriggerTiming.TurnStarted,
                TriggerRelation.Friendly,
                [Sim_FP1_005e.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
