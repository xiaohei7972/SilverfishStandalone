using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 旋风之刃
// Whirling Blades
// 使一个随从获得+1攻击力。
// Give a minion +1 Attack.
internal static class Sim_PART_007
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
