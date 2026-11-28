using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 血蓟毒素
// Bloodthistle Toxin
// 将一个友方随从移回你的手牌。 它的法力值消耗减少（2）点。
// Return a friendly minion to your hand. It costs (2) less.
[CardSim("OG_080c")]
internal static class Sim_OG_080c
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ReturnMinionEffect(-2) { EnchantmentCardId = Sim_OG_080ae.CardId },
        ],
    };
}
