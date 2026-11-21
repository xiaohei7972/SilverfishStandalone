using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 锯齿盾牌
// Serrated Shield
// 获得5点护甲值。造成$5点 伤害。
// Gain 5 Armor. Deal $5 damage.
internal static class Sim_LOOT_285t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
