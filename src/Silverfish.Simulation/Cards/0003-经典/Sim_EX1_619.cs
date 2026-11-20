using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 生而平等
// Equality
// 将所有随从的生命值变为1。
// Change the Health of ALL minions to 1.
[CardSim("EX1_619")]
internal static class Sim_EX1_619
{
    internal static CardRules Rules { get; } = new()
    {
        OnPlay = [Sim_EX1_619e.Rule.ApplyTo(StatTarget.AllMinions)]
    };
}
