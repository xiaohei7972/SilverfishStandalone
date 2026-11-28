using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 祭礼之舞
// Tribute Dance
// 选择一个随从，将其变形成为你另选的一个不同的随从。
// Choose a minion. Choose a different minion to transform it into.
internal static class Sim_DINO_414
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
