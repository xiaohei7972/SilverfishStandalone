using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 更多手臂
// More Arms!
// 使一个随从获得+2/+2。
// Give a minion +2/+2.
internal static class Sim_BOT_219t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
