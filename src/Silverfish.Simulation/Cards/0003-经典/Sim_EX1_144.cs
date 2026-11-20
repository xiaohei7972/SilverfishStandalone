using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 暗影步
// Shadowstep
// 将一个友方随从移回你的手牌，它的法力值消耗减少 （2）点。
// Return a friendly minion to your hand. It_costs (2) less.
[CardSim("EX1_144")]
internal static class Sim_EX1_144
{
    internal static CardRules Rules { get; } = new()
    {
        Target = TargetRequirement.FriendlyMinion,
        OnPlay = [Sim_GBL_002e.Rule.ApplyWhenReturned()]
    };
}
