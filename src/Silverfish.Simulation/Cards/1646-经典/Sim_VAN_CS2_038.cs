using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 先祖之魂
// Ancestral Spirit
// 选择一个随从。当该随从被消灭时，使其返回战场。
// Choose a minion. When that minion is destroyed, return it to the battlefield.
internal static class Sim_VAN_CS2_038
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
