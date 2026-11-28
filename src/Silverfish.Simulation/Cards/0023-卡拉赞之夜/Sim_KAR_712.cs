using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 紫罗兰幻术师
// Violet Illusionist
// 在你的回合中，你的英雄免疫。
// During your turn, your hero is Immune.
[CardSim("KAR_712")]
internal static class Sim_KAR_712
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_KAR_712e.Rule.GrantTo(AuraRecipients.FriendlyHero, AuraCondition.ControllerTurnOnly)],
    };
}
