using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 执刃教徒
// Bladed Cultist
// 连击：获得+1/+1。
// Combo: Gain +1/+1.
[CardSim("OG_070")]
internal static class Sim_OG_070
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ConditionalEffect(new ComboCondition(), [Sim_OG_070e.Rule.ApplyTo(StatTarget.Source)]),
        ],
    };
}
