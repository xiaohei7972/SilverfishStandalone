using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 第一封印
// The First Seal
// 召唤一个2/2的恶魔。将“第二封印”置入你的手牌。
// Summon a 2/2 Demon. Add 'The Second Seal' to your hand.
internal static class Sim_LOOT_415t1
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
