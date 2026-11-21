using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 激光弹幕
// Laser Barrage
// 对一个随从造成$3点伤害。如果你正在构筑星舰，则还会对相邻随从造成伤害。
// Deal $3 damage to a minion. If you're building a Starship, also damage its neighbors.
internal static class Sim_GDB_845
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET), new(ErrorType.REQ_ENEMY_TARGET)],
        []);
    // </legacy-play-requirements>

}
