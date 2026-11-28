using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 沉默
// Silence
// 沉默一个随从。
// Silence a minion.
internal static class Sim_VAN_EX1_332
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
