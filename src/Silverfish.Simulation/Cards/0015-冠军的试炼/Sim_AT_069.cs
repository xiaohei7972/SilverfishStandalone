using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 格斗陪练师
// Sparring Partner
// 嘲讽 战吼：使一个随从获得嘲讽。
// Taunt Battlecry: Give a minion Taunt.
internal static class Sim_AT_069
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_TARGET_IF_AVAILABLE)],
        []);
    // </legacy-play-requirements>

}
