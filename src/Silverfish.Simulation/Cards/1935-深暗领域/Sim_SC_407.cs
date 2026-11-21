using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 锁定
// Lock On
// 将一个随从的生命值变为1。你的下一次星舰发射的法力值消耗减少（2）点。
// Set a minion’s Health to 1. Your next Starship launch costs (2) less.
internal static class Sim_SC_407
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_ENEMY_TARGET), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
