using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 出土神器
// Unearthed Artifacts
// 随机召唤一个法力值消耗为（2）的随从。如果你在本回合中发现过，改为随机召唤一个法力值消耗为（4）的随从。
// Summon a random 2-Cost minion. If you've Discovered this turn, summon a random 4-Cost minion instead.
internal static class Sim_TLC_462
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
