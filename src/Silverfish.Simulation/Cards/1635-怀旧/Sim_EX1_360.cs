using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 谦逊
// Humility
// 使一个随从的攻击力变为1。
// Change a minion's Attack to 1.
[CardSim("EX1_360")]
internal static class Sim_EX1_360
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [Sim_EX1_360e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
