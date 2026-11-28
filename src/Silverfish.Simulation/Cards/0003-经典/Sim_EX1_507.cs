using Silverfish.Domain;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 鱼人领军
// Murloc Warleader
// 你的其他鱼人拥有+2攻击力。
// Your other Murlocs have +2 Attack.
[CardSim("EX1_507")]
internal static class Sim_EX1_507
{
    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_EX1_507e.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions, cardFilter: new() { MinionType = MinionType.Murloc })]
    };
}
