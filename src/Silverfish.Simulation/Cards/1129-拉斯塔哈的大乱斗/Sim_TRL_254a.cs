using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 贡克的坚韧
// Gonk's Resilience
// 使一个随从获得+2/+4和嘲讽。
// Give a minion +2/+4 and Taunt.
internal static class Sim_TRL_254a
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
