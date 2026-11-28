using Silverfish.Domain.Client;
namespace Silverfish.Simulation.Cards;

// 第二封印
// The Second Seal
// 召唤一个3/3的恶魔。将“第三封印”置入你的手牌。
// Summon a 3/3 Demon. Add 'The Third Seal' to your hand.
internal static class Sim_LOOT_415t2
{
    // <legacy-play-requirements>
    internal static PlayReqSet PlayReqs { get; } = new(
        [new(ErrorType.REQ_NUM_MINION_SLOTS, 1)],
        []);
    // </legacy-play-requirements>

}
