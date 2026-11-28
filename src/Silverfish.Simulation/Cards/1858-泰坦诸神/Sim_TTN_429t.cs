using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 塑造星辰
// Shape the Stars
// 选择一个非泰坦随从，召唤一个具有+2/+2的复制。
// Choose a non-Titan minion. Summon a copy of it with +2/+2.
internal static class Sim_TTN_429t
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
