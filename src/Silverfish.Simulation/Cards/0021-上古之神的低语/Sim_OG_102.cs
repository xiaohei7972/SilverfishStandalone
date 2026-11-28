using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 黑暗低语者
// Darkspeaker
// 战吼：与另一个友方随从交换属性值。
// Battlecry: Swap stats with a friendly minion.
[CardSim("OG_102")]
internal static class Sim_OG_102
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new CopyOrSwapMinionStatsEffect(Swap: true, EnchantmentCardId: Sim_OG_102e.CardId),
        ],
    };
}
