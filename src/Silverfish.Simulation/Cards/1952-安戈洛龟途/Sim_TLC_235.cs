using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 生命循环
// Life Cycle
// 消灭一个随从，随机召唤一个法力值消耗相同的随从来替换它。
// Destroy a minion. Summon a random minion of the same Cost to replace it.
internal static class Sim_TLC_235
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_TARGET_TO_PLAY), new(ErrorType.REQ_MINION_TARGET)],
        []);
    // </legacy-play-requirements>

}
