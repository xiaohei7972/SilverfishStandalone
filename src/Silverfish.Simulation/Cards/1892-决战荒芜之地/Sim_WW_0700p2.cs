using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 冰霜枪弹
// Frost Bullet
// 造成$2点伤害。获得$d4点护甲值。每回合切换。
// Deal $2 damage. Gain $d4 Armor. Swaps each turn.
internal static class Sim_WW_0700p2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
