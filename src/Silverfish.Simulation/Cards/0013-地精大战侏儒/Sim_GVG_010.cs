using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 维伦的恩泽
// Velen's Chosen
// 使一个随从获得+2/+4和法术伤害+1。
// Give a minion +2/+4 and Spell Damage +1.
internal static class Sim_GVG_010
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
