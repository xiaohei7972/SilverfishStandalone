using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 震荡弹
// Concussive Shells
// 造成$2点伤害并获得2点护甲值。你的下一次星舰发射的法力值消耗减少（2）点。
// Deal $2 damage and gain 2 Armor. Your next Starship launch costs (2) less.
internal static class Sim_SC_411
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
