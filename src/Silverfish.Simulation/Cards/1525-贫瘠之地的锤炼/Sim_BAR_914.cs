using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 小鬼集群（等级1）
// Imp Swarm (Rank 1)
// 召唤一个3/2的小鬼。（当你有5点法力值时升级。）
// Summon a 3/2 Imp. (Upgrades when you have 5 Mana.)
internal static class Sim_BAR_914
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
