using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 皮糙肉厚
// Thick Hide
// 使一个随从获得+4生命值和 嘲讽。
// Give a minion +4 Health and Taunt.
internal static class Sim_EX1_155b
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
