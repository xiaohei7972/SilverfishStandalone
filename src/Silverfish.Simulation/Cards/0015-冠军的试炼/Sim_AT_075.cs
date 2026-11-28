using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 战马训练师
// Warhorse Trainer
// 你的白银之手新兵拥有+2攻击力和嘲讽。
// Your Silver Hand Recruits have +2 Attack and Taunt.
[CardSim("AT_075")]
internal static class Sim_AT_075
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_AT_075e.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions, cardFilter: new CardFilter { CardId = "CS2_101t" })],
    };
}
