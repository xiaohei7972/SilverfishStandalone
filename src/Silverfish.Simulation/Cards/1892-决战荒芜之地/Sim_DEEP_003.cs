using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 磷光射击
// Shimmer Shot
// 造成$1点伤害，随机召唤一个法力值消耗与伤害量相同的随从。
// Deal $1 damage. Summon a random minion of that Cost.
internal static class Sim_DEEP_003
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY)],
        []);
    // </legacy-play-requirements>

}
