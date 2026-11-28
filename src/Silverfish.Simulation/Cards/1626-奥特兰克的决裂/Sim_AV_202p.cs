using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 巨力猛击
// Grand Slam
// 造成$2点伤害。荣誉消灭：获得$d4点护甲值。
// Deal $2 damage. Honorable Kill: Gain $d4 Armor.
internal static class Sim_AV_202p
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
