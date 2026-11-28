using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 皮糙肉厚
// Thick Hide
// +4生命值并具有嘲讽。
// +4 Health and Taunt.
internal static class Sim_VAN_EX1_155b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
