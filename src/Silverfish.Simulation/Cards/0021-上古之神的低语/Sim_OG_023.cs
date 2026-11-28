using Silverfish.Domain;
using Silverfish.Domain.Client;
using Silverfish.Simulation.Cards.Enchantments;

namespace Silverfish.Simulation.Cards;

// 原始融合
// Primal Fusion
// 你每有一个图腾，就使一个随从获得+1/+1。
// Give a minion +1/+1 for each of your Totems.
[CardSim("OG_023")]
internal static class Sim_OG_023
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

    internal static CardRules Rules { get; } = new()
    {
        OnPlay =
        [
            new ScaledEnchantStatsEffect(StatTarget.Selected, StatScaleSource.FriendlyMinions, 1, 1, Sim_OG_023t.CardId) { CardFilter = new CardFilter { MinionType = MinionType.Totem } },
        ],
    };
}
