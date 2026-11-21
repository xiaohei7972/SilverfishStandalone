using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 简单维修
// Small Repairs
// 使一个随从获得+2生命值和 嘲讽。
// Give a minion +2 Health and Taunt.
internal static class Sim_DRG_311b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
