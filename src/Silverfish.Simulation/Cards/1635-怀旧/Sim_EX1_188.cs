using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 贫瘠之地饲养员
// Barrens Stablehand
// 战吼：随机召唤一只野兽。
// Battlecry: Summon a random Beast.
internal static class Sim_EX1_188
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
