using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 乘风浮龙
// Windpeak Wyrm
// 战吼：造成5点伤害，获得5点护甲值。延系：法力值消耗减少（2）点。
// Battlecry: Deal 5 damage and gain 5 Armor. Kindred: Costs (2) less.
internal static class Sim_TLC_600
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
