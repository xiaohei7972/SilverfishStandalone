using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 战争古树
// Ancient of War
// 抉择： +5攻击力；或者+5生命值并具有嘲讽。
// Choose One - +5 Attack; or +5 Health and Taunt.
[CardSim("EX1_178")]
internal static class Sim_EX1_178
{
    internal static CardRules Rules { get; } = new()
    {
        Choices =
        [
            new("EX1_178a", [Sim_EX1_178ae.Rule.ApplyTo(StatTarget.Source)]),
            new("EX1_178b", [Sim_EX1_178be.Rule.ApplyTo(StatTarget.Source)])
        ]
    };
}
