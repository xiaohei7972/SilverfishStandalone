using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 光速
// Lightspeed
// 回响 使一个随从获得+1/+2和突袭。
// Echo Give a minion +1/+2 and Rush.
internal static class Sim_GDB_457
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
