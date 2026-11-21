using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 已腐蚀的梦魇
// Corrupted Nightmare
// 使一个随从获得+5/+5和在本回合中的免疫。
// Give a minion Immune this turn and +5/+5.
internal static class Sim_EDR_846t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_FRIENDLY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
