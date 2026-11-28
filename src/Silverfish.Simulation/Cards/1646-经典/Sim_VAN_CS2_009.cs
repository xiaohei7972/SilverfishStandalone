using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野性印记
// Mark of the Wild
// 使一个随从获得嘲讽和+2/+2。（+2攻击力/+2生命值）
// Give a minion Taunt and +2/+2. (+2 Attack/+2 Health)
internal static class Sim_VAN_CS2_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
