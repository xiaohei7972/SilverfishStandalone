using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 野性印记
// Mark of the Wild
// 使一个随从获得嘲讽和+2/+3。（+2攻击力/+3生命值）
// Give a minion Taunt and +2/+3. (+2 Attack/+3 Health)
internal static class Sim_CORE_CS2_009
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
