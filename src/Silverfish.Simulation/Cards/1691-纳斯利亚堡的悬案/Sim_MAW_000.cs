using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冒牌小鬼
// Imp-oster
// 战吼：选择一个友方小鬼，变形成为一个它的复制。
// Battlecry: Choose a friendly Imp. Transform into a copy of it.
internal static class Sim_MAW_000
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_IF_AVAILABLE), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_MUST_HAVE_TAG, GAME_TAG.IMP)],
        []);
    // </legacy-play-requirements>

}
