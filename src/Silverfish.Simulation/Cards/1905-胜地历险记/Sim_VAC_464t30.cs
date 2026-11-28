using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 奎尔德拉的剑柄
// Hilt of Quel'Delar
// 使一个随从获得+3/+3。
// Give a minion +3/+3.
internal static class Sim_VAC_464t30
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
