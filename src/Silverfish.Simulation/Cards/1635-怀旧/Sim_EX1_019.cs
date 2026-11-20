using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 破碎残阳祭司
// Shattered Sun Cleric
// 战吼：使一个友方随从获得+1/+1。
// Battlecry: Give a friendly minion +1/+1.
[CardSim("EX1_019")]
internal static class Sim_EX1_019
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [Sim_EX1_019e.Rule.ApplyTo(StatTarget.Selected)]
    };
}
