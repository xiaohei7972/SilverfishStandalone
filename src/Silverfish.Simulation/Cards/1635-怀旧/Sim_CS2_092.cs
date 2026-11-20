using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 王者祝福
// Blessing of Kings
// 使一个随从获得+4/+4。（+4攻击力/+4生命值）
// Give a minion +4/+4. (+4 Attack/+4 Health)
[CardSim("CS2_092")]
internal static class Sim_CS2_092
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.Minion,
        OnPlay = [Sim_CS2_092e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
