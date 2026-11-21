using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 变异注射
// Mutating Injection
// 使一个随从获得+4/+4和嘲讽。
// Give a minion +4/+4 and Taunt.
internal static class Sim_VAC_464t3
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
