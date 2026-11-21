using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 雷欧克
// Leokk
// 你的其他随从拥有+1攻击力。
// Your other minions have +1 Attack.
[CardSim("NEW1_033")]
internal static class Sim_NEW1_033
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        Auras = [Sim_NEW1_033o.Rule.GrantTo(AuraRecipients.OtherFriendlyMinions)]
    };
}
