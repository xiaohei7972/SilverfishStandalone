using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 保护之手
// Hand of Protection
// 使一个随从获得圣盾。
// Give a minion Divine Shield.
internal static class Sim_VAN_EX1_371
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
