using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 虚空行者
// Voidwalker
// 嘲讽
// Taunt
internal static class Sim_CS2_065
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
