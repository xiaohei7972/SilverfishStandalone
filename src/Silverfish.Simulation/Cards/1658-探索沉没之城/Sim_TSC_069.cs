using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 深海融合怪
// Amalgam of the Deep
// 战吼：选择一个友方随从，发现一张相同类型的随从牌。
// Battlecry: Choose a friendly minion. Discover a minion of the same minion type.
internal static class Sim_TSC_069
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.CARDRACE), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
