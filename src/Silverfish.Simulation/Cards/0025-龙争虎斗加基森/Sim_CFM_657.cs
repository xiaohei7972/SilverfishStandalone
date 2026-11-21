using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 暗金教窃歌者
// Kabal Songstealer
// 战吼： 沉默一个随从。
// Battlecry: Silence a minion.
internal static class Sim_CFM_657
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NONSELF_TARGET), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
