using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 隐藏
// Conceal
// 直到你的下个回合，使所有友方随从获得潜行。
// Give your minions Stealth until your next_turn.
[CardSim("EX1_128")]
internal static class Sim_EX1_128
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_128e.Rule.ApplyTo(StatTarget.FriendlyMinions)]
    };
}
